using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomProductNameResolverRequest : IValueResolver<Request, RequestListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomProductNameResolverRequest(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Request source, RequestListDTO destination, string destMember, ResolutionContext context)
        {
            var package = _context.Packages.FirstOrDefault(r => r.Id == source.PackageId);
            var product = _context.Products.FirstOrDefault(r => r.Id == package.ProductId);
            return product != null ? product.Name : null;
        }
    }
}
