using BeeStore_Repository.DTO.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.Batch
{
    public class BatchListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Status { get; set; }

        public DateTime? CompleteDate { get; set; }

        public int? DeliveryZoneId { get; set; }

        public List<OrderListDTO> Orders { get; set; }
    }
}
