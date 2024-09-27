using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<Pagination<WarehouseListDTO>> GetWarehouseList(int pageIndex, int pageSize);
        Task<WarehouseCreateDTO> CreateWarehouse(WarehouseCreateDTO request);
        Task<WarehouseCreateDTO> UpdateWarehouse(WarehouseCreateDTO request);
        Task<string> DeleteWarehouse(int id);
    }
}
