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
    public class PackageService : IPackageService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public PackageService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreatePackage(PackageCreateDTO request)
        {
            if (request.InventoryId != null)
            {
                var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.InventoryId);
                if (inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
            }
            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }


            var result = _mapper.Map<Package>(request);
            var productCategory = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == product.ProductCategoryId);

            result.CreateDate = DateTime.Now;
            result.ExpirationDate = DateTime.Now.AddDays(productCategory.ExpireIn!.Value);
            await _unitOfWork.PackageRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeletePackage(int id)
        {
            var exist = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            _unitOfWork.PackageRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<PackageListDTO> GetPackageById(int id)
        {
            var exist = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            var result = _mapper.Map<PackageListDTO>(exist);
            return result;
        }

        public async Task<Pagination<PackageListDTO>> GetPackageList(PackageFilter? filterBy, string? filterQuery, PackageSortBy? sortBy,
                                                                     bool descending, int pageIndex, int pageSize)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            Expression<Func<Package, bool>> filterExpression = null;
            switch (filterBy){
                case PackageFilter.ProductId: filterExpression = u => u.ProductId.Equals(Int32.Parse(filterQuery!)); break;
                case PackageFilter.InventoryId: filterExpression = u => u.InventoryId.Equals(Int32.Parse(filterQuery!)); break;
                default: filterExpression = null; break;
            }


            string? sortCriteria = sortBy switch
            {
                PackageSortBy.Amount => Constants.SortCriteria.Amount,
                PackageSortBy.ExpirationDate => Constants.SortCriteria.ExpirationDate,
                PackageSortBy.CreateDate => Constants.SortCriteria.CreateDate,
                PackageSortBy.ProductAmount => Constants.SortCriteria.ProductAmount,
                _ => null
            };

            var list = await _unitOfWork.PackageRepo.GetListAsync(
                filter: filterExpression!,
                includes: null,
                sortBy: sortCriteria!,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
         
            var result = _mapper.Map<List<PackageListDTO>>(list);
            return (await ListPagination<PackageListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<string> UpdatePackage(int id, PackageCreateDTO request)
        {
            var exist = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            if (request.InventoryId != null)
            {
                var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.InventoryId);
                if (inventory == null)
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
            _unitOfWork.PackageRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
