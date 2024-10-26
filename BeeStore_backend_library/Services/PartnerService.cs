using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public PartnerService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Pagination<PartnerListDTO>> GetAllPartners(string search, SortBy? sortby, bool descending, int pageIndex, int pageSize)
        {
            string sortCriteria = sortby.ToString()!;

            var list = await _unitOfWork.OcopPartnerRepo.GetListAsync(
                filter: null!,
                includes: o => o.Include(o => o.Province)
                                .Include(o => o.Category)
                                .Include(o => o.OcopCategory)
                                .Include(o => o.Role),
                sortBy: sortCriteria!,
                descending: descending,
                searchTerm: search!,
                searchProperties: new Expression<Func<OcopPartner, string>>[] { p => p.Email, p => p.FirstName,
                                                                                p => p.LastName, p => p.BusinessName}
                );
                
            var result = _mapper.Map<List<PartnerListDTO>>(list);

            return await ListPagination<PartnerListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<PartnerListDTO> GetPartner(string email)
        {
            var partner = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == email,
                                                                               query => query.Include(o => o.Province)
                                                                                             .Include(o => o.Category)
                                                                                             .Include(o => o.OcopCategory)
                                                                                             .Include(o => o.Role));
            if (partner == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }
            var result = _mapper.Map<PartnerListDTO>(partner);
            return result;
        }

        

        public async Task<string> UpdatePartner(OCOPPartnerUpdateRequest user)
        {
            var exist = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Email == user.Email);
            if (exist != null)
            {
                if (BCrypt.Net.BCrypt.Verify(user.ConfirmPassword, exist.Password))
                {
                    throw new ApplicationException(ResponseMessage.UserPasswordError);
                }
                if ((DateTime.Now - exist.UpdateDate.Value).TotalDays < 30)
                {
                    throw new ApplicationException(ResponseMessage.UpdatePartnerError);
                }

                exist.Setting = user.Setting;
                exist.PictureLink = user.PictureLink;
                if (!String.IsNullOrEmpty(user.Phone) && !user.Phone.Equals(Constants.DefaultString.String))
                {
                    exist.Phone = user.Phone;
                }
                if (!String.IsNullOrEmpty(user.FirstName) && !user.FirstName.Equals(Constants.DefaultString.String))
                {
                    exist.FirstName = user.FirstName;
                }
                if (!String.IsNullOrEmpty(user.LastName) && !user.LastName.Equals(Constants.DefaultString.String))
                {
                    exist.LastName = user.LastName;
                }
                exist.BankAccountNumber = user.BankAccountNumber;
                exist.BankName = user.BankName;
                exist.BusinessName = user.BusinessName;
                exist.CategoryId = user.CategoryId;
                exist.ProvinceId = user.ProvinceId;
                exist.TaxIdentificationNumber = user.TaxIdentificationNumber;
                exist.OcopCategoryId = user.OcopCategoryId;
                exist.UpdateDate = DateTime.Now;
                _unitOfWork.OcopPartnerRepo.Update(exist);
                _unitOfWork.SaveAsync();
                return ResponseMessage.Success;
            }
            else
            {
                throw new KeyNotFoundException(ResponseMessage.UserEmailNotFound);
            }

        }

        public async Task<string> DeletePartner(int id)
        {
            var exist = await _unitOfWork.PartnerRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PartnerIdNotFound);
            }
            _unitOfWork.PartnerRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
