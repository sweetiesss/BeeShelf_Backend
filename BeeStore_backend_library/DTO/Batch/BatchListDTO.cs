using BeeStore_Repository.DTO.OrderDTOs;


namespace BeeStore_Repository.DTO.Batch
{
    public class BatchListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Status { get; set; }

        public DateTime? CompleteDate { get; set; }
        public DateTime? DeliveryStartDate { get; set; }

        public int? DeliverBy { get; set; }
        public string? ShipperEmail { get; set; }
        public string? ShipperName { get; set; }

        public int? DeliveryZoneId { get; set; }
        public string? DeliveryZoneName { get; set; }

        public List<OrderListDTO> Orders { get; set; }
    }

}
