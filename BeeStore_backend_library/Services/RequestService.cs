using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;

namespace BeeStore_Repository.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public RequestService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CancelRequest(int id, string cancellationReason)
        {
            var request = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (request == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!request.Status.Equals(Constants.Status.Draft) || !request.Status.Equals(Constants.Status.Pending))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            request.CancellationReason = cancellationReason;
            request.Status = Constants.Status.Canceled;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> SendRequest(int id)
        {
            var request = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (request == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!request.Status.Equals(Constants.Status.Draft))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            request.Status = Constants.Status.Pending;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }


        public async Task<string> CreateRequest(RequestType type, bool Send, RequestCreateDTO request)
        {



            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == request.OcopPartnerId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var userInventory = await _unitOfWork.InventoryRepo.AnyAsync(u => u.Id.Equals(inventory.Id)
                                                                           && u.OcopPartner.Id.Equals(user.Id));
            if (userInventory == false)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryPartnerNotMatch);
            }


            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.Lot.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }
            var userProduct = await _unitOfWork.ProductRepo.AnyAsync(u => u.Id.Equals(product.Id)
                                                                       && u.OcopPartnerId.Equals(user.Id));
            if (userProduct == false)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductPartnerNotMatch);
            }

            decimal? totalWeight = inventory.Weight + product.Weight * request.Lot.ProductAmount;


            if (totalWeight > inventory.MaxWeight)
            {
                throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
            }
            request.RequestType = type.ToString();
            if (Send == true)
            {
                request.Status = Constants.Status.Pending;
            }
            else
            {
                request.Status = Constants.Status.Draft;
            }
            var result = _mapper.Map<Request>(request);

            //result.Lot.InventoryId = request.SendToInventoryId;

            await _unitOfWork.RequestRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteRequest(int id)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            _unitOfWork.RequestRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<List<Request>> ApplyFilterToList(RequestStatus? requestStatus, bool descending,
                                                          int? userId = null, int? warehouseId = null)
        {
            string? filterQuery = requestStatus switch
            {
                RequestStatus.Draft => Constants.Status.Draft,
                RequestStatus.Pending => Constants.Status.Pending,
                RequestStatus.Canceled => Constants.Status.Canceled,
                RequestStatus.Processing => Constants.Status.Processing,
                RequestStatus.Delivered => Constants.Status.Delivered,
                RequestStatus.Completed => Constants.Status.Completed,
                RequestStatus.Failed => Constants.Status.Failed,
                _ => null
            };




            var list = await _unitOfWork.RequestRepo.GetListAsync(
                filter: u => (filterQuery == null || u.Status.Equals(filterQuery))
                             && (userId == null || u.OcopPartnerId.Equals(userId))
                             && (warehouseId == null || u.SendToInventory.WarehouseId.Equals(warehouseId)),
                includes: null,
                sortBy: Constants.SortCriteria.CreateDate,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
            return list;
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(RequestStatus? status, bool descending, int warehouseId, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(status, descending, null, warehouseId);
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(int userId, RequestStatus? status, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(status, descending, userId);
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }


        public async Task<string> UpdateRequest(int id, RequestCreateDTO request)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!exist.Status.Equals(Constants.Status.Draft))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            var user = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == request.OcopPartnerId);
            if (user == false)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if (exist.Lot.InventoryId != request.SendToInventoryId)
            {
                exist.Lot.InventoryId = request.SendToInventoryId;
            }
            exist.Lot.ProductId = request.Lot.ProductId;
            exist.Lot.Amount = request.Lot.Amount;
            exist.Lot.LotNumber = request.Lot.LotNumber;
            exist.Lot.Name = request.Lot.Name;
            exist.Lot.ProductAmount = request.Lot.ProductAmount;

            exist.SendToInventoryId = request.SendToInventoryId;
            exist.Description = request.Description;
            _unitOfWork.RequestRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }


        public async Task<string> UpdateRequestStatus(int id, RequestStatus status)
        {

            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!exist.Status.Equals(Constants.Status.Pending))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }

            string? requestStatus = status switch
            {
                RequestStatus.Draft => string.Empty,
                RequestStatus.Pending => string.Empty,
                RequestStatus.Canceled => string.Empty,
                RequestStatus.Processing => Constants.Status.Processing,
                RequestStatus.Delivered => Constants.Status.Delivered,
                RequestStatus.Completed => Constants.Status.Completed,
                RequestStatus.Failed => Constants.Status.Failed,
                _ => string.Empty
            };
            if (requestStatus.Equals(string.Empty))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }



            if (requestStatus == Constants.Status.Completed)
            {
                var lot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.LotId));
                if (lot == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                }
                var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.SendToInventoryId));
                if (inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
                var totalWeight = inventory.Weight + (lot.Product.Weight * lot.ProductAmount);
                if (totalWeight > inventory.MaxWeight)
                {
                    exist.Status = Constants.Status.Failed;
                    exist.CancellationReason = ResponseMessage.InventoryOverWeightError;
                    await _unitOfWork.SaveAsync();
                    throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
                }

                lot.InventoryId = exist.SendToInventoryId;
                lot.ExpirationDate = DateTime.Now.AddDays(lot.Product.ProductCategory!.ExpireIn!.Value);

                inventory.Weight = totalWeight;

            }

            exist.Status = requestStatus;
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<RequestListDTO>(exist);
            return ResponseMessage.Success;
        }
    }
}
