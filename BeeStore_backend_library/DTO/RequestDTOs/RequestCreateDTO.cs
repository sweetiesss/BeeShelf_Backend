using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.RequestDTOs
{
    public class RequestCreateDTO
    {
        public int? UserId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? PackageId { get; set; }

        public int? SendToInventory { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [JsonIgnore]
        public string? Status { get; set; }

        public string? RequestType { get; set; }
    }
}
