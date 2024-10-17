using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomUserEmailResolverRequest : IValueResolver<Request, RequestListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomUserEmailResolverRequest(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Request source, RequestListDTO destination, string destMember, ResolutionContext context)
        {
            var user = _context.Users.FirstOrDefault(r => r.Id == source.UserId);
            return user != null ? user.Email : null;
        }
    }
}
