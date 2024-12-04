using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.VehicleDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<Pagination<VehicleListDTO>> GetVehicles(VehicleStatus? status, VehicleType? type,
                                                     VehicleSortBy? sortCriteria, bool descending, int pageIndex, int pageSize, int? warehouseId);
        Task<VehicleListDTO> GetVehicle(int id);
        Task<VehicleListDTO> GetShipperVehicle(int shipperId);

        Task<string> CreateVehicle(VehicleType? type, VehicleCreateDTO request);
        Task<string> UpdateVehicle(int id, VehicleType? type, VehicleCreateDTO request);
        Task<string> DeleteVehicle(int id);
        Task<string> UpdateVehicleStatus(int id, VehicleStatus? status);
        Task<string> AssignVehicle(int id, int driver_id);
    }
}
