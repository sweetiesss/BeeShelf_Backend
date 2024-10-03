using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.WarehouseCategoryDTOs
{
    public class WarehouseCategoryCreateDTO
    {
        public int? WarehouseId { get; set; }
        public int? ProductCategoryId { get; set; }
    }
}
