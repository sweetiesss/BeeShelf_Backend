using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.ProductCategoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public ProductCategoryService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ProductCategoryCreateDTO> CreateProductCategory(ProductCategoryCreateDTO request)
        {
            var exist = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.TypeName == request.TypeName);
            if (exist != null)
            {
                if(exist.IsDeleted == true)
                {
                    exist.TypeName = null;
                    _unitOfWork.ProductCategoryRepo.Update(exist);
                    await _unitOfWork.SaveAsync();
                }
                else
                {
                    throw new DuplicateException(ResponseMessage.ProductCategoryDuplicate);
                }
            }
            var result = _mapper.Map<ProductCategory>(request);
            await _unitOfWork.ProductCategoryRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<string> DeleteProductCategory(int id)
        {
            var exist = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new DuplicateException(ResponseMessage.ProductCategoryIdNotFound);
            }
            _unitOfWork.ProductCategoryRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<Pagination<ProductCategoryListDTO>> GetProductCategoryList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.ProductCategoryRepo.GetAllAsync();
            var result = _mapper.Map<List<ProductCategoryListDTO>>(list);
            return (await ListPagination<ProductCategoryListDTO>.PaginateList(result, pageIndex, pageSize));

        }

        public async Task<ProductCategoryCreateDTO> UpdateProductCategory(int id, ProductCategoryCreateDTO request)
        {

            var exist = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.TypeName == request.TypeName);
            if (exist != null)
            {
                if (exist.IsDeleted == true)
                {
                    exist.TypeName = null;
                    _unitOfWork.ProductCategoryRepo.Update(exist);
                    await _unitOfWork.SaveAsync();
                }
                else if(exist.Id != id)
                {
                    throw new DuplicateException(ResponseMessage.ProductCategoryDuplicate);
                }
            }
            var prodCat = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id == id);
            if(prodCat == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductCategoryIdNotFound);
            }
            prodCat.TypeName = request.TypeName;
            prodCat.TypeDescription = request.TypeDescription;
            prodCat.ExpireIn = request.ExpireIn;
            _unitOfWork.ProductCategoryRepo.Update(prodCat);
            await _unitOfWork.SaveAsync();
            return request;
        }
    }
}
