﻿using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.OrderDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomUserEmailResolverOrder : IValueResolver<Order, OrderListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomUserEmailResolverOrder(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Order source, OrderListDTO destination, string destMember, ResolutionContext context)
        {
            var user = _context.Users.FirstOrDefault(r => r.Id == source.UserId);
            return user != null ? user.Email : null;
        }
    }
}
