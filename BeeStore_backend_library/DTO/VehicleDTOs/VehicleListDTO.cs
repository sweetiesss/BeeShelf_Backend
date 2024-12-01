namespace BeeStore_Repository.DTO.VehicleDTOs
{
    public class VehicleListDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? LicensePlate { get; set; }

        public decimal? Capacity { get; set; }

        public string? Type { get; set; }
        public ulong? IsCold { get; set; }
        public string? Status { get; set; }
        public int? WarehouseId { get; set; }

        public int? AssignedDriverId { get; set; }
        public string? AssignedDriverEmail { get; set; }
        public string? AssignedDriverName { get; set; }
    }
}
