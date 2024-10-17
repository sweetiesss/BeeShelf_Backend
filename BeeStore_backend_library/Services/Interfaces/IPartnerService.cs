using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<Pagination<PartnerListDTO>> GetPartnerList(int pageIndex, int pageSize);
        Task<string> UpgradeToPartner(PartnerUpdateRequest request);
        Task<string> UpdatePartner(PartnerUpdateRequest request);
        Task<string> DeletePartner(int id);
    }
}
