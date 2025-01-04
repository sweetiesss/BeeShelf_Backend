namespace BeeStore_Repository.DTO.ProvinceDTOs
{
    public class ProvinceListDTO
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public string? SubDivisionName { get; set; }

        public List<DZDTO> DeliveryZones { get; set; }
    }

    public class DZDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? ProvinceId { get; set; }

    }
}
