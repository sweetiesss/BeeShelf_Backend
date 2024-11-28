using BeeStore_Repository.DTO.OrderDTOs;


namespace BeeStore_Repository.DTO.Batch
{
    public class BatchListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Status { get; set; }

        public DateTime? CompleteDate { get; set; }

        public int? DeliverBy { get; set; }

        public int? DeliveryZoneId { get; set; }

        public List<OrderListDTO> BatchDeliveries { get; set; }
    }

    public class BatchDeliveriesListDTO
    {
        public int Id { get; set; }

        public int? NumberOfTrips { get; set; }

        public DateTime? DeliveryStartDate { get; set; }

        public int? BatchId { get; set; }
        public virtual List<OrderListDTO> Orders { get; set; } = new List<OrderListDTO>();

    }
}
