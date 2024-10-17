using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.WarehouseCategoryDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    internal class CustomProductCategoryResolverWarehouseCategory : IValueResolver<WarehouseCategory, WarehouseCategoryListDTO, string?>
    {
        private readonly BeeStoreDbContext _context;

        public CustomProductCategoryResolverWarehouseCategory(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(WarehouseCategory source, WarehouseCategoryListDTO destination, string destMember, ResolutionContext context)
        {
            var proCat = _context.ProductCategories.FirstOrDefault(r => r.Id == source.ProductCategoryId);
            return proCat != null ? proCat.TypeName : null;
        }
    }
}
