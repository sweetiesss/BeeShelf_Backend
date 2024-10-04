using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.InventoryDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    internal class CustomWarehouseNameResolverWarehouseShipper : IValueResolver<WarehouseShipper, WarehouseShipperListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomWarehouseNameResolverWarehouseShipper(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(WarehouseShipper source, WarehouseShipperListDTO destination, string destMember, ResolutionContext context)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(r => r.Id == source.WarehouseId);
            return warehouse != null ? warehouse.Name : null;
        }
    }
}
