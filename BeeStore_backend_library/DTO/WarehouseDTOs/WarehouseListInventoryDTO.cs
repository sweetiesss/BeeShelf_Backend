﻿using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.WarehouseDTOs
{
    public class WarehouseListInventoryDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal? Size { get; set; }

        public string? Location { get; set; }
        public int TotalInventory { get; set; }
        public DateTime? CreateDate { get; set; }
        public List<InventoryListDTO> Inventories { get; set; }
    }

}

