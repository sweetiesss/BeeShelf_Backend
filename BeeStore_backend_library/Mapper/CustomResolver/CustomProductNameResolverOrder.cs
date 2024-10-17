using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomProductNameResolverOrder : IValueResolver<Order, OrderListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomProductNameResolverOrder(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Order source, OrderListDTO destination, string destMember, ResolutionContext context)
        {
            var product = _context.Products.FirstOrDefault(r => r.Id == source.ProductId);
            return product != null ? product.Name : null;
        }
    }
}
