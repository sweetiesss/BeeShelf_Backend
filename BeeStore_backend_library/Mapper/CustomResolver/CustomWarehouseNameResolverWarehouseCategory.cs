using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomWarehouseNameResolverWarehouseCategory : IValueResolver<WarehouseCategory, WarehouseCategoryListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomWarehouseNameResolverWarehouseCategory(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(WarehouseCategory source, WarehouseCategoryListDTO destination, string destMember, ResolutionContext context)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(r => r.Id == source.WarehouseId);
            return warehouse != null ? warehouse.Name : null;
        }
    }
}
