using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
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
                throw new KeyNotFoundException("User not found.");
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventory);
            if(inventory == null)
            {
                throw new KeyNotFoundException("Inventory not found.");
            }
            var package = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id == request.PackageId);
            if(package == null)
            {
                throw new KeyNotFoundException("Package not found.");
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
                throw new KeyNotFoundException("Request not found.");
            }
            _unitOfWork.RequestRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return "Success";
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
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventory);
            if (inventory == null)
            {
                throw new KeyNotFoundException("Inventory not found.");
            }
            var package = await _unitOfWork.PackageRepo.SingleOrDefaultAsync(u => u.Id == request.PackageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Package not found.");
            }
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if(exist == null)
            {
                throw new KeyNotFoundException("Request not found.");
            }
            if(exist.Status != "Pending")
            {
                throw new ApplicationException("This request has already been processed.");
            }
            exist.SendToInventory = request.SendToInventory;
            exist.Description = request.Description;
            exist.Name = request.Name;
            exist.PackageId = request.PackageId;
            exist.RequestType = request.RequestType;
            _unitOfWork.RequestRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;
        }
    }
}
