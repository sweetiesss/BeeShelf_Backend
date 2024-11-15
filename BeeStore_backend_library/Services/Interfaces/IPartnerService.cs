using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Enums.SortBy;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<Pagination<PartnerListDTO>> GetAllPartners(string search,SortBy? sortby, bool descending, int pageIndex, int pageSize);
        Task<PartnerListDTO> GetPartner(string email);
        Task<string> UpdatePartner(OCOPPartnerUpdateRequest request);
        Task<string> DeletePartner(int id);

        Task<List<PartnerRevenueDTO>> GetPartnerRevenue(int id, int? day, int? month, int?  year);
    }
}
