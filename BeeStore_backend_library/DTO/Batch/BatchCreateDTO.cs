using BeeStore_Repository.DTO.OrderDTOs;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.Batch
{
    public class BatchCreateDTO
    {
        public string? Name { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }

        public int? DeliveryZoneId { get; set; }

        public List<BatchOrdersCreate> Orders { get; set; }
    }

    public class BatchOrdersCreate
    {
        public int? Id { get; set; }
        [JsonIgnore]
        public int? BatchId { get; set; }
    }
}
