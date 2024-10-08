using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<PackageCreateDTO> CreatePackage(PackageCreateDTO request)
        {
            if (request.InventoryId != null)
            {
                var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.InventoryId);
                if(inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
            }
            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.ProductId); 
            if(product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }

            
            var result = _mapper.Map<Package>(request);
            var productCategory = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == product.ProductCategoryId);

            result.CreateDate = DateTime.Now;
            result.ExpirationDate = DateTime.Now.AddDays(productCategory.ExpireIn.Value);
            await _unitOfWork.PackageRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
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

        public async Task<Pagination<PackageListDTO>> GetPackageList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.PackageRepo.GetAllAsync();
            var result = _mapper.Map<List<PackageListDTO>>(list);
            return (await ListPagination<PackageListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<PackageCreateDTO> UpdatePackage(int id, PackageCreateDTO request)
        {
            var exist = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id ==  id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            if(request.InventoryId != null)
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
            exist.Weight = request.Weight;
            exist.ProductAmount = request.ProductAmount;
            exist.ProductId = request.ProductId;
            exist.InventoryId = request.InventoryId;
            exist.ExpirationDate = DateTime.Now.AddDays(productCategory.ExpireIn.Value);
            _unitOfWork.PackageRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;
        }
    }
}
