using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomRoleNameReverseResolver : IValueResolver<UserCreateRequestDTO, User, int?>
    {
        private readonly BeeStoreDbContext _context;

        public CustomRoleNameReverseResolver(BeeStoreDbContext context)
        {
            _context = context;
        }

        //public int Resolve(UserListDTO source, User destination, int? destMember, ResolutionContext context)
        //{
        //    var role = _context.Roles.FirstOrDefault(r => r.RoleName == source.RoleName);
        //    return role != null ? role.Id : default;
        //}

        int? IValueResolver<UserCreateRequestDTO, User, int?>.Resolve(UserCreateRequestDTO source, User destination, int? destMember, ResolutionContext context)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == source.RoleName);
            return role != null ? role.Id : default;
        }
    }
}
