using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.PackageDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomProductNameResolverPackage : IValueResolver<Package, PackageListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomProductNameResolverPackage(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Package source, PackageListDTO destination, string destMember, ResolutionContext context)
        {
            var product = _context.Products.FirstOrDefault(r => r.Id == source.ProductId);
            return product != null ? product.Name : null;
        }
    }

}
