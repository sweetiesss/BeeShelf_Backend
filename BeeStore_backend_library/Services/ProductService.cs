using Amazon.Runtime.EventStreams.Internal;
using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections;
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
            var exist = await _unitOfWork.ProductRepo.FirstOrDefaultAsync(u => u.Name == request.Name && u.UserId == request.UserId);
            if (exist != null)
            {
                if(exist.IsDeleted == false)
                {
                    throw new DuplicateException("A product with this name already exist.");
                }
                if(exist.IsDeleted == true)
                {
                    exist.Name = null;
                    _unitOfWork.ProductRepo.Update(exist);
                    await _unitOfWork.SaveAsync();
                }
            }
            request.CreateDate = DateTime.Now;
            request.ExpirationDate = null;
            var result = _mapper.Map<Product>(request);
            await _unitOfWork.ProductRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<List<ProductCreateDTO>> CreateProductRange(List<ProductCreateDTO> request)
        {
            StringBuilder sb = new StringBuilder();
            string error = string.Empty;

            //check dupes in list
            var dupes = request.Select((x, i) => new { index = i, value = x })
                   .GroupBy(x => new { x.value.Name})
                   .Where(x => x.Skip(1).Any());

            if (dupes.Any())
            {
                foreach(var group in dupes)
                {
                    sb.Append($"{group.Key.Name}, ");
                }
                error = sb.ToString();
                if (!String.IsNullOrEmpty(error))
                {
                    throw new DuplicateException("Please check provided list. The following name is duplicate: " + error);
                }
            }
            
            //proces request list
            foreach (var item in request)
            {
                var exist = await _unitOfWork.ProductRepo.FirstOrDefaultAsync(u => u.Name == item.Name && u.UserId == item.UserId);
                if (exist != null && exist.UserId == item.UserId)
                {
                    if(exist.IsDeleted == true)
                    {
                        exist.Name = null;
                        _unitOfWork.ProductRepo.Update(exist);
                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        sb.Append($"{item.Name}, ");
                    }
                }
            }
            error = sb.ToString();
            if (!String.IsNullOrEmpty(error))
            {
                    throw new DuplicateException("Failed to add. Product with these name already exist in your account: " + error);
            }
            var result = _mapper.Map<List<Product>>(request);
            await _unitOfWork.ProductRepo.AddRangeAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<string> DeleteProduct(int id)
        {
            var exist = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException("Product does not exist.");
            }
                _unitOfWork.ProductRepo.SoftDelete(exist);
                await _unitOfWork.SaveAsync();
                return "Success";

        }

        public async Task<Pagination<ProductListDTO>> GetProductList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.ProductRepo.GetAllAsync();
            var result = _mapper.Map<List<ProductListDTO>>(list);
            return (await ListPagination<ProductListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<ProductListDTO>> GetProductListByEmail(string email, int pageIndex, int pageSize)
        {
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == email);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            var list = await _unitOfWork.ProductRepo.GetFiltered(u => u.UserId.Equals(user.Id));
            var result = _mapper.Map<List<ProductListDTO>>(list);
            return (await ListPagination<ProductListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<ProductCreateDTO> UpdateProduct(int id, ProductCreateDTO request)
        {
            //Check if the product exist or not
            var exist = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == id);
            if(exist == null || exist.IsDeleted == true)
            {
                throw new KeyNotFoundException("This product doesn't exist or has already been deleted.");
            }


            //Check if the product's user Id match with the request user Id
            if (exist.UserId != request.UserId)
            {
                throw new AppException("User misamatched.");
            }

            //Check for duplicate name
            //If a product with duplicate name exist that has the same email as the request
            //Then check if that product has the same id with the request and its deleted status
            //If deleted status is false and it doesn't have the same id then it's a duplicate name
            //If deleted status is false and it has the same id then it's fine to update
            //If deleted status is true then update the name of that duplicate product to null
            //Then proceed to update
            
            var duplicateName = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Name == request.Name && u.UserId == request.UserId);

            if (duplicateName != null) 
            {
                if (duplicateName.Id != id && duplicateName.IsDeleted == false)
                {
                    throw new DuplicateException("A product with this name already exist.");
                }

                if (duplicateName.IsDeleted == true)
                {
                    duplicateName.Name = null;
                    _unitOfWork.ProductRepo.Update(duplicateName);
                    await _unitOfWork.SaveAsync();
                }
            }

            exist.Name = request.Name;
            exist.Origin = request.Origin;
            exist.Weight = request.Weight;
            exist.Price = request.Price;
            exist.ExpirationDate = null; //NOT IMPLEMENTED
            exist.PictureId = request.PictureId;
            exist.ProductCategoryId = request.ProductCategoryId;
            exist.ProductAmount = request.ProductAmount;
            _unitOfWork.ProductRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;
        }
    }
}
