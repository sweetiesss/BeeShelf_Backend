using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly HashSet<string> _existingInventoryNames = new HashSet<string>();
        public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
       
        public async Task<string> CreateRoom(RoomCreateDTO request)
        {
            var exist = await _unitOfWork.StoreRepo.GetByIdAsync(request.StoreId);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }
            decimal? totalWeight = 0;
            decimal? storeWeight = exist.Rooms.Sum(o => o.MaxWeight).Value;
            //if(request.InventoryAmount > 0)
            //{
            //    totalWeight = request.InventoryAmount * request.MaxWeight;
            //}
            //else
            //{
            //    throw new ApplicationException(ResponseMessage.BadRequest);
            //}
            foreach(var x in request.data)
            {
                totalWeight += x.MaxWeight;
                if(exist.Rooms.Any(u => u.RoomCode.Equals(x.RoomCode)))
                {
                    throw new ApplicationException(ResponseMessage.InventoryNameDuplicate);
                }
                exist.Rooms.Add(new Room
                {
                    RoomCode = x.RoomCode,
                    MaxWeight = x.MaxWeight,
                    Weight = 0,
                    Price = x.Price,
                    StoreId = request.StoreId,
                    Width = x.Width,
                    Length = x.Length,
                    X = x.X,
                    Y = x.Y,
                    IsCold = x.IsCold,
                });
            }

            if ((totalWeight + storeWeight) > exist.Capacity)
            {
                throw new ApplicationException(ResponseMessage.WarehouseAddInventoryCapacityLimitReach);
            }

            //Check inv capacity with total capacity of warehouse



            //for (int i = 0; i < request.InventoryAmount; i++)
            //{
            //    string inventoryName;
            //    int attempts = 0;
            //    const int maxAttempts = 100; 

            //    do
            //    {
            //        inventoryName = GenerateUniqueInventoryName();
            //        attempts++;

            //        if (attempts >= maxAttempts)
            //        {
            //            throw new ApplicationException();
            //        }
            //    }
            //    while (_existingInventoryNames.Contains(inventoryName) ||
            //           exist.Rooms.Any(inv => inv.RoomCode == inventoryName && inv.StoreId.Equals(request.WarehouseId)));


            //    _existingInventoryNames.Add(inventoryName);


            //}


            //_unitOfWork.WarehouseRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteRoom(int id)
        {
            var exist = await _unitOfWork.RoomRepo.GetByIdAsync(id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            _unitOfWork.RoomRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateRoom(RoomUpdateDTO request)
        {

            var exist = await _unitOfWork.RoomRepo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (DateTime.Now > exist.BoughtDate && DateTime.Now < exist.ExpirationDate)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryOccupied);
            }
            if (request.MaxWeight != null && request.MaxWeight != 0)
            {
                exist.MaxWeight = request.MaxWeight;
            }
            exist.IsCold = request.IsCold;
            exist.Price = request.Price;
            exist.Width = request.Width;
            exist.Length = request.Length;
            exist.X = request.X;
            exist.Y = request.Y;
            _unitOfWork.RoomRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;

        }

        private async Task<List<Room>> ApplyFilterToList(RoomFilter? filterBy, string? filterQuery,
                                                          RoomSortBy? sortCriteria,
                                                          bool descending, int? userId = null)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }
            Expression<Func<Room, bool>> filterExpression = u =>
            (userId == null || u.OcopPartnerId.Equals(userId)) &&
            (filterBy == null || (filterBy == RoomFilter.StoreId && u.StoreId.Equals(Int32.Parse(filterQuery!))));


            string? sortBy = sortCriteria switch
            {
                RoomSortBy.BoughtDate => Constants.SortCriteria.BoughtDate,
                RoomSortBy.ExpirationDate => Constants.SortCriteria.ExpirationDate,
                RoomSortBy.Name => Constants.SortCriteria.Name,
                RoomSortBy.MaxWeight => Constants.SortCriteria.MaxWeight,
                RoomSortBy.Weight => Constants.SortCriteria.Weight,
                _ => null
            };

            var list = await _unitOfWork.RoomRepo.GetListAsync(
                filter: filterExpression,
                includes: u => u.Include(o => o.Lots),
                sortBy: sortBy!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
            return list;
        }

        public async Task<Pagination<RoomListDTO>> GetRoomList(RoomFilter? filterBy, string filterQuery,
                                                          RoomSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(filterBy, filterQuery, sortCriteria, descending);
            var result = _mapper.Map<List<RoomListDTO>>(list);
            return (await ListPagination<RoomListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<RoomListDTO>> GetRoomList(int userId, RoomFilter? filterBy, string filterQuery,
                                                          RoomSortBy? sortCriteria,
                                                          bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(filterBy, filterQuery, sortCriteria, descending, userId);


            var result = _mapper.Map<List<RoomListDTO>>(list);
            return (await ListPagination<RoomListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<RoomLotListDTO> GetRoomById(int id)
        {
            var exist = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var result = _mapper.Map<RoomLotListDTO>(exist);
            return result;
        }

        public async Task<string> BuyRoom(int id, int userId, int month)
        {
            if (month <= 0)
            {
                throw new ApplicationException(ResponseMessage.BadRequest);
            }
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == userId,
                                                                              query => query.Include(o => o.Wallets));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inv = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (inv == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (inv.OcopPartnerId != null && inv.OcopPartnerId != userId)
            {
                throw new ApplicationException(ResponseMessage.InventoryOccupied);
            }
            var wallet = user.Wallets.FirstOrDefault(u => u.OcopPartnerId == userId);
            if (wallet.TotalAmount < inv.Price * month)
            {
                throw new ApplicationException(ResponseMessage.NotEnoughCredit);
            }
            inv.Transactions.Add(new Transaction
            {
                Code = GenerateRandomLetters(),
                CreateDate = DateTime.Now,
                Description = $"{user.LastName} bought {month} of {inv.RoomCode}",
                Amount = (int)(inv.Price * month),
                RoomId = inv.Id,
                MonthAmount = month,
                OcopPartnerId = userId,
                IsDeleted = false,
                Status = Constants.PaymentStatus.Paid
            });
            wallet.TotalAmount -= inv.Price * month;
            inv.OcopPartnerId = userId;
            inv.BoughtDate = DateTime.Now;
            inv.ExpirationDate = DateTime.Now.AddMonths(month);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> ExtendRoom(int id, int userId, int month)
        {
            if (month == 0)
            {
                throw new ApplicationException(ResponseMessage.BadRequest);
            }
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == userId,
                                                                              query => query.Include(o => o.Wallets));
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inv = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (inv == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (inv.OcopPartnerId != userId)
            {
                throw new ApplicationException(ResponseMessage.InventoryPartnerNotMatch);
            }
            var wallet = user.Wallets.FirstOrDefault(u => u.OcopPartnerId == userId);
            if (wallet.TotalAmount < inv.Price * month)
            {
                throw new ApplicationException(ResponseMessage.NotEnoughCredit);
            }
            inv.ExpirationDate = inv.ExpirationDate.Value.AddMonths(month);
            wallet.TotalAmount -= inv.Price * month;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private string GenerateUniqueInventoryName()
        {
            // Generate a random 5-digit number
            return $"ROOM-{Random.Shared.Next(10000, 99999):D5}";
        }

        private string GenerateRandomLetters(int length = 4)
        {
            Random random = new Random();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            char[] randomLetters = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomLetters[i] = letters[random.Next(letters.Length)];
            }

            return new string(randomLetters);
        }
    }
}
