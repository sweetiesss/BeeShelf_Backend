using BeeStore_Repository.DTO;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.DTO.VehicleDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<Pagination<VehicleListDTO>> GetVehicles(VehicleStatus? status, VehicleType? type, 
                                                     VehicleSortBy? sortCriteria, bool descending, int pageIndex, int pageSize);
        Task<VehicleListDTO> GetVehicle(int id);
        Task<string> CreateVehicle(VehicleType? type, VehicleCreateDTO request);
        Task<string> UpdateVehicle(int id, VehicleType? type, VehicleCreateDTO request);
        Task<string> DeleteVehicle(int id);
        Task<string> UpdateVehicleStatus(int id, VehicleStatus? status);
        Task<string> AssignVehicle(int id, int driver_id);
    }
}
