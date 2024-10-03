using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IWarehouseStaffService
    {
        Task<Pagination<WarehouseStaffListDTO>> GetWarehouseStaffList(int pageIndex, int pageSize);
    }
}
