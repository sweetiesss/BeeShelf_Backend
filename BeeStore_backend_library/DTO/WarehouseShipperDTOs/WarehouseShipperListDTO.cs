using BeeStore_Repository.DTO.VehicleDTOs;

namespace BeeStore_Repository.DTO.WarehouseShipperDTOs
{
    public class WarehouseShipperListDTO
    {

        public int? EmployeeId { get; set; }
        public string email { get; set; }
        public string? ShipperName { get; set; }
        public string? Status { get; set; }
        public int? DeliveryZoneId { get; set; }
        public string? DeliveryZoneName { get; set; }
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public List<VehicleListDTO> Vehicles { get; set; }
    }
}
