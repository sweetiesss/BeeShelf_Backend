using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseShipperService
    {
        Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(int pageIndex, int pageSize);
        Task<Pagination<WarehouseShipperListDTO>> GetWarehouseShipperList(int id, int pageIndex, int pageSize);
        Task<string> AddShipperToWarehouse(List<WarehouseShipperCreateDTO> request);


    }
}
