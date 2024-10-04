using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.WarehouseStaffDTOs
{
    public class WarehouseStaffCreateDTO
    {
        public int? UserId { get; set; }

        public int? WarehouseId { get; set; }
    }
}
