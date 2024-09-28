using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
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
    public class ProductService : IProductService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public ProductService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductCreateDTO> CreateProduct(ProductCreateDTO request)
        {
            var exist = await _unitOfWork.ProductRepo.FirstOrDefaultAsync(u => u.Name == request.Name && u.PartnerEmail == request.PartnerEmail);
            if (exist != null)
            {
                throw new DuplicateException("A product with this name already exist.");
            }
            var result = _mapper.Map<Product>(request);
            await _unitOfWork.ProductRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<List<ProductCreateDTO>> CreateProductRange(List<ProductCreateDTO> request)
        {
            StringBuilder sb = new StringBuilder();
            string error = string.Empty;
            foreach(var item in request)
            {
                var exist = await _unitOfWork.ProductRepo.FirstOrDefaultAsync(u => u.Name == item.Name && u.PartnerEmail == item.PartnerEmail);
                if (exist != null && exist.PartnerEmail == item.PartnerEmail)
                {
                  sb.Append($"A product with `{item.Name}` name already exist. ");
                }
            }
            error = sb.ToString();
            if (!String.IsNullOrEmpty(error))
            {
                    throw new DuplicateException(error);
            }
            var result = _mapper.Map<List<Product>>(request);
            await _unitOfWork.ProductRepo.AddRangeAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public Task<string> DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<ProductListDTO>> GetProductList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.ProductRepo.GetAllAsync();
            var result = _mapper.Map<List<ProductListDTO>>(list);
            return (await ListPagination<ProductListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public Task<ProductCreateDTO> UpdateProduct(ProductCreateDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
