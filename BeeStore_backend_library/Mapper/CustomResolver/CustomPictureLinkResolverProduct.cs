using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomPictureLinkResolverProduct : IValueResolver<Product, ProductListDTO, string?>
    {
        private readonly BeeStoreDbContext _context;

        public CustomPictureLinkResolverProduct(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Product source, ProductListDTO destination, string destMember, ResolutionContext context)
        {
            var picture = _context.Pictures.FirstOrDefault(r => r.Id == source.PictureId);
            return picture != null ? picture.PictureLink : null;
        }
    }
}
