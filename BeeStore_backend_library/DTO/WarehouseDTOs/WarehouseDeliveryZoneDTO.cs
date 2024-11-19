﻿namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseDeliveryZoneDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Capacity { get; set; }

        public string? Location { get; set; }

        public DateTime? CreateDate { get; set; }
        public List<DeliveryZoneDTO> DeliveryZones { get; set; }
    }

    public class DeliveryZoneDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

    }
}
