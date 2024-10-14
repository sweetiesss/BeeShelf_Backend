using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class RequestService : IRequestService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public RequestService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RequestCreateDTO> CreateRequest(RequestCreateDTO request)
        {
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.UserId);
            if(user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventory);
            if(inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }

            var package = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id.Equals(request.PackageId),
                                                                                query => query.Include(o => o.Product)
                                                                                .ThenInclude(item => item.ProductCategory));
            if (package == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }

            var totalWeight = inventory.Weight + (package.Product.Weight * package.ProductAmount);
            if (totalWeight > inventory.MaxWeight)
            {
                throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
            }
            request.Status = "Pending";
            var result = _mapper.Map<Request>(request);
            await _unitOfWork.RequestRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<string> DeleteRequest(int id)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if(exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            _unitOfWork.RequestRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.RequestRepo.GetAllAsync();
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(string email, int pageIndex, int pageSize)
        {
            var requests = await _unitOfWork.RequestRepo.GetQueryable(query => query.Include(o => o.User));
            var list = requests.Where(u => u.User.Email == email).ToList();
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<RequestCreateDTO> UpdateRequest(int id, RequestCreateDTO request)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventory);
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var package = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id == request.PackageId);
            if (package == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            if(exist.Status != "Pending")
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            exist.SendToInventory = request.SendToInventory;
            exist.Description = request.Description;
            exist.PackageId = request.PackageId;
            exist.RequestType = request.RequestType;
            _unitOfWork.RequestRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<RequestListDTO> UpdateRequestStatus(int id, int statusId)
        {
            string status = null;
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (exist.Status != "Pending")
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            if (statusId == 1)
            {
                status = "Approved.";
                var package = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.PackageId),
                                                                                 query => query.Include(o => o.Product)
                                                                                 .ThenInclude(item => item.ProductCategory));
                if(package == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                }
                var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.SendToInventory));
                if (inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
                var totalWeight = inventory.Weight + (package.Product.Weight * package.ProductAmount);
                if (totalWeight > inventory.MaxWeight)
                {
                    throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
                }

                package.InventoryId = exist.SendToInventory;
                package.ExpirationDate = DateTime.Now.AddDays(package.Product.ProductCategory.ExpireIn.Value);

                inventory.Weight = totalWeight;

            }
            if (statusId == 2)
            {
                status = "Reject";
            }
            exist.Status = status;
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<RequestListDTO>(exist);
            return result;
        }
    }
}
