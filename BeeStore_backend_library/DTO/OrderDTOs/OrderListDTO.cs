﻿namespace BeeStore_Repository.DTO.OrderDTOs
{
    public class OrderListDTO
    {

        public int Id { get; set; }
        public string? OrderCode { get; set; }
        public string? partner_email { get; set; }

        public string? Status { get; set; }

        public string? CancellationReason { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? ApproveDate { get; set; }

        public DateTime? DeliverStartDate { get; set; }

        public DateTime? DeliverFinishDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public string? ReceiverName { get; set; }

        public string? ReceiverPhone { get; set; }

        public string? ReceiverAddress { get; set; }
        public decimal? Distance { get; set; }
        public int? WarehouseID { get; set; }
        public string? WarehouseName { get; set; }
        public string? WarehouseLocation { get; set; }
        public int? DeliveryZoneId { get; set; }
        public string? DeliveryZoneName { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPriceAfterFee { get; set; }
        public int? BatchId { get; set; }
        public int? NumberOfTrips { get; set; }

        public string? PictureLink { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
        public List<OrderFeeDTO> OrderFees { get; set; }

    }

    public class OrderFeeDTO
    {
        public decimal? DeliveryFee { get; set; }

        public decimal? StorageFee { get; set; }

        public decimal? AdditionalFee { get; set; }

    }

    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public int? LotId { get; set; }
        public string? LotName { get; set; }
        public int? ProductId { get; set; }

        public string? ProductName { get; set; }

        public decimal? ProductPrice { get; set; }
        public string? Unit { get; set; }
        public string? Weight { get; set; }

        public int? ProductAmount { get; set; }
        public string? ProductImage { get; set; }
        public int? RoomId {  get; set; }
        public string? RoomCode { get; set;}
    }
}
