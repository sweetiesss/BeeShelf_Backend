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

    public class CustomRoleNameResolver : IValueResolver<User, UserListDTO, string>
        {
            private readonly BeeStoreDbContext _context;

            public CustomRoleNameResolver(BeeStoreDbContext context)
            {
                _context = context;
            }

            public string Resolve(User source, UserListDTO destination, string destMember, ResolutionContext context)
            {
                var role = _context.Roles.FirstOrDefault(r => r.Id == source.RoleId);
                return role != null ? role.RoleName : null;
            }
        }
    }
