using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderUpdateDTO
    {
        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }
        [JsonIgnore]
        public string? Status { get; set; }
        [JsonIgnore]
        public decimal? TotalPrice { get; set; }
        public List<OrderDetailUpdateDTO> OrderDetails { get; set; }

    }

    public class OrderDetailUpdateDTO
    {

        public int? LotId { get; set; }

        [JsonIgnore]
        public int? ProductPrice { get; set; }

        public int? ProductAmount { get; set; }
    }
}
