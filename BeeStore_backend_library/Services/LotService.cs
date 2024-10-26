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
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class LotService : ILotService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public LotService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateLot(LotCreateDTO request)
        {
            if (request.InventoryId != null)
            {
                var inventory = await _unitOfWork.InventoryRepo.AnyAsync(u => u.Id == request.InventoryId);
                if (inventory == false)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
            }
            var product = await _unitOfWork.ProductRepo.AnyAsync(u => u.Id == request.ProductId);
            if (product == false)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }


            var result = _mapper.Map<Lot>(request);
            
            //this should be in requet (after imported start counting expiration date) or not idk
            result.ExpirationDate = DateTime.Now.AddDays(result.Product.ProductCategory.ExpireIn!.Value);
            await _unitOfWork.LotRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
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
            switch (filterBy){
                case LotFilter.ProductId: filterExpression = u => u.ProductId.Equals(Int32.Parse(filterQuery!)); break;
                case LotFilter.InventoryId: filterExpression = u => u.InventoryId.Equals(Int32.Parse(filterQuery!)); break;
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
                includes: null,
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

            if(!await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id.Equals(partnerId)))
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }

            Expression<Func<Lot, bool>> filterExpression = null;
            switch (filterBy)
            {
                case LotFilter.ProductId: filterExpression = u => u.ProductId.Equals(Int32.Parse(filterQuery!))
                                                               && u.Inventory.OcopPartnerId.Equals(partnerId); break;
                case LotFilter.InventoryId: filterExpression = u => u.InventoryId.Equals(Int32.Parse(filterQuery!))
                                                               && u.Inventory.OcopPartnerId.Equals(partnerId); break;
                default: filterExpression = u => u.Inventory.OcopPartnerId.Equals(partnerId); break;
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
                includes: null,
                sortBy: sortCriteria!,
                descending: descending,
                searchTerm: search,
                searchProperties: new Expression<Func<Lot, string>>[] { o => o.LotNumber, o => o.Name }
                );

            var result = _mapper.Map<List<LotListDTO>>(list);
            return (await ListPagination<LotListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<string> UpdateLot(int id, LotCreateDTO request)
        {
            var exist = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            if (request.InventoryId != null)
            {
                var inventory = await _unitOfWork.InventoryRepo.AnyAsync(u => u.Id == request.InventoryId);
                if (inventory == false)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
            }
            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }
            var productCategory = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == product.ProductCategoryId);
            exist.ProductAmount = request.ProductAmount;
            exist.ProductId = request.ProductId;
            exist.InventoryId = request.InventoryId;
            exist.ExpirationDate = DateTime.Now.AddDays(productCategory.ExpireIn!.Value);
            _unitOfWork.LotRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
