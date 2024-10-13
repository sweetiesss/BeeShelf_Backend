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

namespace BeeStore_Repository.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public WarehouseService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WarehouseCreateDTO> CreateWarehouse(WarehouseCreateDTO request)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Name == request.Name);
            if (exist != null)
            {
                throw new DuplicateException("Warehouse with this name already exists.");
            }
            var warehouse = _mapper.Map<Warehouse>(request);
            warehouse.CreateDate = DateTime.Now;
            await _unitOfWork.WarehouseRepo.AddAsync(warehouse);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<string> DeleteWarehouse(int id)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException("No warehouse found.");
            }
            _unitOfWork.WarehouseRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return "Success";
        }


        public async Task<List<WarehouseListInventoryDTO>> GetWarehouseByUserId(int userId)
        {
            var query = await _unitOfWork.WarehouseRepo.GetQueryable(q => q
                .Include(w => w.Inventories)
                //.ThenInclude(i => i.User)
            );

            var result = query
                .Where(w => w.Inventories.Any(i => i.UserId == userId && !i.IsDeleted))
                .Select(w => new WarehouseListInventoryDTO
                {
                    Id = w.Id,
                    Name = w.Name,
                    Size = w.Size,
                    Location = w.Location,
                    CreateDate = w.CreateDate,
                    TotalInventory = w.Inventories.Count(i => i.UserId == userId && !i.IsDeleted),
                    Inventories = w.Inventories
                        .Where(i => i.User.Id == userId && !i.IsDeleted)
                        .Select(i => new InventoryListDTO
                        {
                            Id = i.Id,
                            UserId = i.UserId,
                            Name = i.Name,
                            MaxWeight = i.MaxWeight,
                            Weight = i.Weight,
                            BoughtDate = i.BoughtDate,
                            WarehouseName = w.Name,
                            ExpirationDate = i.ExpirationDate
                        }).ToList()
                })
                .ToList();

            return result;
        }


        public async Task<Pagination<WarehouseListDTO>> GetWarehouseList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.WarehouseRepo.GetAllAsync();
            var result = _mapper.Map<List<WarehouseListDTO>>(list);

            return await ListPagination<WarehouseListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<WarehouseCreateDTO> UpdateWarehouse(WarehouseCreateDTO request)
        {
            var exist = await _unitOfWork.WarehouseRepo.SingleOrDefaultAsync(u => u.Name == request.Name);
            if (exist == null)
            {
                throw new DuplicateException("No warehouse with this name found.");
            }

            if (!String.IsNullOrEmpty(request.Location) && !request.Location.Equals("string"))
            {
                exist.Location = request.Location;
            }
            if (request.Size != null && request.Size != 0)
            {
                exist.Size = request.Size;
            }

            _unitOfWork.WarehouseRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;

        }
    }
}
