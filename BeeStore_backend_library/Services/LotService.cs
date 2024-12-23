using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class LotService : ILotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public LotService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> DeleteLot(int id)
        {
            var exist = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            _unitOfWork.LotRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<LotListDTO> GetLotById(int id)
        {
            var exist = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            var result = _mapper.Map<LotListDTO>(exist);
            return result;
        }

        public async Task<Pagination<LotListDTO>> GetAllLots(string search, LotFilter? filterBy, string? filterQuery, LotSortBy? sortBy,
                                                                     bool descending, int pageIndex, int pageSize)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            Expression<Func<Lot, bool>> filterExpression = null;
            switch (filterBy)
            {
                case LotFilter.ProductId: filterExpression = u => u.ProductId.Equals(Int32.Parse(filterQuery!)); break;
                case LotFilter.RoomId: filterExpression = u => u.RoomId.Equals(Int32.Parse(filterQuery!)); break;
                default: filterExpression = null; break;
            }


            string? sortCriteria = sortBy switch
            {
                LotSortBy.Amount => Constants.SortCriteria.Amount,
                LotSortBy.ExpirationDate => Constants.SortCriteria.ExpirationDate,
                LotSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                LotSortBy.ProductAmount => Constants.SortCriteria.ProductAmount,
                _ => null
            };

            var list = await _unitOfWork.LotRepo.GetListAsync(
                filter: filterExpression!,
                includes: u => u.Include(o => o.Product)
                                .Include(o => o.Room).ThenInclude(o => o.Store),
                sortBy: sortCriteria!,
                descending: descending,
                searchTerm: search,
                searchProperties: new Expression<Func<Lot, string>>[] { o => o.LotNumber, o => o.Name }
                );

            var result = _mapper.Map<List<LotListDTO>>(list);
            return (await ListPagination<LotListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<LotListDTO>> GetLotsByUserId(int partnerId, string search, LotFilter? filterBy, string? filterQuery, LotSortBy? sortBy,
                                                                     bool descending, int pageIndex, int pageSize)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            if (!await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id.Equals(partnerId)))
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }

            Expression<Func<Lot, bool>> filterExpression = null;
            switch (filterBy)
            {
                case LotFilter.ProductId:
                    filterExpression = u => u.ProductId.Equals(Int32.Parse(filterQuery!))
                                                               && u.Room.OcopPartnerId.Equals(partnerId)
                                                               && u.RequestLots.Any(u => u.Status.Equals(Constants.Status.Completed)); break;
                case LotFilter.RoomId:
                    filterExpression = u => u.RoomId.Equals(Int32.Parse(filterQuery!))
                                                               && u.Room.OcopPartnerId.Equals(partnerId)
                                                               && u.RequestLots.Any(u => u.Status.Equals(Constants.Status.Completed)); break;
                default:
                    filterExpression = u => u.Room.OcopPartnerId.Equals(partnerId)
                                              && u.RequestLots.Any(u => u.Status.Equals(Constants.Status.Completed)); break;
            }


            string? sortCriteria = sortBy switch
            {
                LotSortBy.Amount => Constants.SortCriteria.Amount,
                LotSortBy.ExpirationDate => Constants.SortCriteria.ExpirationDate,
                LotSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                LotSortBy.ProductAmount => Constants.SortCriteria.ProductAmount,
                _ => null
            };

            var list = await _unitOfWork.LotRepo.GetListAsync(
                filter: filterExpression!,
                includes: u => u.Include(o => o.Product)
                                .Include(o => o.Room).ThenInclude(o => o.Store),
                sortBy: sortCriteria!,
                descending: descending,
                searchTerm: search,
                searchProperties: new Expression<Func<Lot, string>>[] { o => o.LotNumber, o => o.Name }
                );

            var result = _mapper.Map<List<LotListDTO>>(list);
            return (await ListPagination<LotListDTO>.PaginateList(result, pageIndex, pageSize));
        }

    
    }
}
