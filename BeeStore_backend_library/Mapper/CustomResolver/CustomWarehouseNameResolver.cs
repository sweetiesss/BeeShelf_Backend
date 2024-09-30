using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomWarehouseNameResolver : IValueResolver<Inventory, InventoryListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomWarehouseNameResolver(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Inventory source, InventoryListDTO destination, string destMember, ResolutionContext context)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(r => r.Id == source.WarehouseId);
            return warehouse != null ? warehouse.Name : null;
        }
    }
}
