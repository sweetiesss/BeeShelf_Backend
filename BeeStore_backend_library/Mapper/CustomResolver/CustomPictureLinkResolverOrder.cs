using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.DTO.ProductDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomPictureLinkResolverOrder : IValueResolver<Order, OrderListDTO, string?>
    {
        private readonly BeeStoreDbContext _context;

        public CustomPictureLinkResolverOrder(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Order source, OrderListDTO destination, string destMember, ResolutionContext context)
        {
            var picture = _context.Pictures.FirstOrDefault(r => r.Id == source.PictureId);
            return picture != null ? picture.PictureLink : null;
        }
    }
}
