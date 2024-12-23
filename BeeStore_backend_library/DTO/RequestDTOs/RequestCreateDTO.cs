using BeeStore_Repository.DTO.PackageDTOs;
using System.Text.Json.Serialization;

namespace BeeStore_Repository.DTO.RequestDTOs
{
    public class RequestCreateDTO
    {

        public int? OcopPartnerId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        [JsonIgnore]
        public string? RequestType { get; set; }

        public int? ExportFromLotId { get; set; }
        public int? SendToRoomId { get; set; }

        public LotCreateDTO? Lot { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public string? Status { get; set; }

    }
}
