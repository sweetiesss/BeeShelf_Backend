using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using BeeStore_Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseShipperService
    {
        Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(int pageIndex, int pageSize);
        Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(int id, int pageIndex, int pageSize);
        Task<List<WarehouseShipperCreateDTO>> AddShipperToWarehouse(List<WarehouseShipperCreateDTO> request);


    }
}
