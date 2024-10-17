﻿namespace BeeStore_Repository.DTO.InventoryDTOs
{
    public class InventoryUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? MaxWeight { get; set; }

        public decimal? Weight { get; set; }

        //public DateTime? BoughtDate { get; set; }

        //public DateTime? ExpirationDate { get; set; }

    }
}
