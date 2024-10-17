using AutoMapper;
using BeeStore_Repository.Data;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Models;

namespace BeeStore_Repository.Mapper.CustomResolver
{
    public class CustomUserEmailResolverPartner : IValueResolver<Partner, PartnerListDTO, string>
    {
        private readonly BeeStoreDbContext _context;

        public CustomUserEmailResolverPartner(BeeStoreDbContext context)
        {
            _context = context;
        }

        public string Resolve(Partner source, PartnerListDTO destination, string destMember, ResolutionContext context)
        {
            var user = _context.Users.FirstOrDefault(r => r.Id == source.UserId);
            return user != null ? user.Email : null;
        }
    }
}
