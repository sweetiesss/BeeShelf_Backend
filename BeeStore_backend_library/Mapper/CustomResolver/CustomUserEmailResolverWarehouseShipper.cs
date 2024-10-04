using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    internal class CustomUserEmailResolverWarehouseShipper : IValueResolver<WarehouseShipper, WarehouseShipperListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomUserEmailResolverWarehouseShipper(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(WarehouseShipper source, WarehouseShipperListDTO destination, string destMember, ResolutionContext context)
        {
            var user = _context.Users.FirstOrDefault(r => r.Id == source.UserId);
            return user != null ? user.Email : null;
        }
    }
}