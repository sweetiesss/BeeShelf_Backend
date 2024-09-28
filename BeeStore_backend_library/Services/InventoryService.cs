using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.WarehouseDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
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
        public async Task<InventoryListDTO> AddPartnerToInventory(int id, string email)
        {
            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(id);
            if (exist == null)
            {
                throw new KeyNotFoundException("Inventory not found.");
            }
            if(exist.PartnerEmail != null)
            {
                throw new DuplicateException("This inventory is already occupied.");
            }
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            exist.PartnerEmail = user.Email;
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
                throw new KeyNotFoundException("Warehouse not found.");
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
                throw new KeyNotFoundException("Inventory not found.");
            }
            _unitOfWork.InventoryRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return "Success";
        }

        public async Task<InventoryUpdateDTO> UpdateInventory(InventoryUpdateDTO request)
        {

            var exist = await _unitOfWork.InventoryRepo.GetByIdAsync(request.Id);
            if (exist == null)
            {
                throw new KeyNotFoundException("Inventory not found.");
            }
            if(request.Weight != null)
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
    }
}
