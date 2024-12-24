using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.ProvinceDTOs;
using BeeStore_Repository.Enums.SortBy;
using Microsoft.AspNetCore.Http;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<Pagination<PartnerListDTO>> GetAllPartners(string search, SortBy? sortby, bool descending, int pageIndex, int pageSize);
        Task<PartnerListDTO> GetPartner(string email);
        Task<string> UpdatePartner(OCOPPartnerUpdateRequest request);
        Task<string> DeletePartner(int id);
        Task<List<ProvinceListDTO>> GetProvince();
        Task<List<PartnerRevenueDTO>> GetPartnerRevenue(int id, int? year);
        Task<PartnerProductDTO> GetPartnerTotalProduct(int id, int? warehouseId);
        Task<string> CreatePartnerVerificationPaper(int ocop_partner_id, List<IFormFile> file);
        Task<PartnerVerificationPaperDTO> GetPartnerVerificationPaper(int partnerId);
        Task<Pagination<PartnerVerificationPaperDTO>> GetAllPartnerVerificationPaper(bool? verified, int pageIndex, int pageSize);

        Task<string> VerifyPartnerVerificationPaper(int partnerVerPaperid);
        Task<string> RejectPartnerVerificationPaper(int partnerVerPaperid, string reason);

    }
}
