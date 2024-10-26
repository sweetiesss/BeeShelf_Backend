using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.UserDTOs.Interfaces;
using BeeStore_Repository.Models;
using BeeStore_Repository.Utils;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomRoleNameResolver<TSource> : IValueResolver<TSource, Employee, int?>
        where TSource : IRoleNameProvider
    {
        private readonly BeeStoreDbContext _context;

        public CustomRoleNameResolver(BeeStoreDbContext context)
        {
            _context = context;
        }


        int? IValueResolver<TSource, Employee, int?>.Resolve(TSource source, Employee destination, int? destMember, ResolutionContext context)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == source.RoleName);
            if (role.RoleName == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RoleNotFound);
            }
            return role != null ? role.Id : default;
        }
    }
}
