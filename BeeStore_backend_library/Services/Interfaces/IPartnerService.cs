using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services.Interfaces
{
    public interface IPartnerService
    {
        Task<Pagination<PartnerListDTO>> GetPartnerList(int pageIndex, int pageSize);
        Task<UpgradeToPartnerRequest> UpgradeToPartner(UpgradeToPartnerRequest request);
        Task<PartnerUpdateRequest> UpdatePartner(PartnerUpdateRequest request);
        Task<string> DeletePartner(string email);
    }
}
