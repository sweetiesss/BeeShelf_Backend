using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public InventoryService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<InventoryListDTO> AddPartnerToInventory(int id, int userId)
        {
            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            if(exist.UserId != null)
            {
                throw new DuplicateException(ResponseMessage.InventoryOccupied);
            }
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (user.RoleId != 4)
            {
                throw new KeyNotFoundException(ResponseMessage.UserRoleNotPartnerError);
            }

            exist.UserId = user.Id;
            _unitOfWork.InventoryRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<InventoryListDTO>(exist);
            return result;
        }


        public async Task<InventoryCreateDTO> CreateInventory(InventoryCreateDTO request)
        {
            var exist = await _unitOfWork.WarehouseRepo.GetByIdAsync(request.WarehouseId);
            if(exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.WarehouseIdNotFound);
            }
            var dupe = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Name.Equals(request.Name,
                                                                            StringComparison.OrdinalIgnoreCase));
            if (dupe != null)
            {
                throw new ApplicationException(ResponseMessage.InventoryNameDuplicate);
            }
            var result = _mapper.Map<Inventory>(request);
            result.BoughtDate = DateTime.Now;
            result.ExpirationDate = DateTime.Now;
            await _unitOfWork.InventoryRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return request;

        }

        public async Task<string> DeleteInventory(int id)
        {
            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            _unitOfWork.InventoryRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<InventoryUpdateDTO> UpdateInventory(InventoryUpdateDTO request)
        {

            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var dupe = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Name.Equals(request.Name,
                                                                            StringComparison.OrdinalIgnoreCase));
            if (dupe != null)
            {
                throw new ApplicationException(ResponseMessage.InventoryNameDuplicate);
            }
            if (request.Weight != null)
            {
                exist.Weight = request.Weight;
            }
            if(request.MaxWeight != null && request.MaxWeight != 0)
            {
                exist.MaxWeight = request.MaxWeight;
            }
            _unitOfWork.InventoryRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;

        }

        public async Task<Pagination<InventoryListDTO>> GetInventoryList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.InventoryRepo.GetAllAsync();
            var result = _mapper.Map<List<InventoryListDTO>>(list);
            return (await ListPagination<InventoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<InventoryListDTO>> GetInventoryList(int userId, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.InventoryRepo.GetFiltered(u => u.UserId.Equals(userId));
            
            var result = _mapper.Map<List<InventoryListDTO>>(list);
            return (await ListPagination<InventoryListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<InventoryListPackagesDTO> GetInventoryById(int id)
        {
            var exist = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if(exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var result = _mapper.Map<InventoryListPackagesDTO>(exist);
            return result;
        }
    }
}
