using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.DTO.UserDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Logger.GlobalExceptionHandler.CustomException;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<User> UserExist(string email)
        {
            Expression<Func<User, bool>> keySelector = u => u.Email == email;
            var exist = await _unitOfWork.UserRepo.GetByKeyAsync(email, keySelector);
            if (exist == null)
            {
            return null;
            }
            return exist;
        }

        public async Task<Partner> PartnerExist(string email)
        {
            Expression<Func<Partner, bool>> keySelector = u => u.UserEmail == email;
            var exist = await _unitOfWork.PartnerRepo.GetByKeyAsync(email, keySelector);
            if (exist == null)
            {
                return null;
            }
            return exist;
        }

        public async Task<Pagination<PartnerListDTO>> GetPartnerList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.PartnerRepo.GetAllAsync();
            var result = _mapper.Map<List<PartnerListDTO>>(list);
            
            return await ListPagination<PartnerListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<UpgradeToPartnerRequest> UpgradeToPartner(UpgradeToPartnerRequest request)
        {
            if(await UserExist(request.UserEmail) == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            
            if(await PartnerExist(request.UserEmail) != null)
            {
                throw new DuplicateException("User is already a partner");
            }
            request.CreateDate = DateTime.Now;
            request.UpdateDate = DateTime.Now;
            var partner = _mapper.Map<Partner>(request);
            await _unitOfWork.PartnerRepo.AddAsync(partner);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<PartnerUpdateRequest> UpdatePartner(PartnerUpdateRequest request)
        {
            var exist = await PartnerExist(request.UserEmail);
            if(exist == null)
            {
                throw new KeyNotFoundException("Partner does not exist");
            }
            exist.UpdateDate = DateTime.Now;
            if (!String.IsNullOrEmpty(request.BankAccountNumber) && !request.BankAccountNumber.Equals("string"))
            { 
                exist.BankAccountNumber = request.BankAccountNumber;
            }
            if (!String.IsNullOrEmpty(request.CitizenIdentificationNumber) && !request.CitizenIdentificationNumber.Equals("string"))
            {
                exist.CitizenIdentificationNumber = request.CitizenIdentificationNumber;
            }
            if (!String.IsNullOrEmpty(request.BankName) && !request.BankName.Equals("string"))
            {
                exist.BankName = request.BankName;
            }

            _unitOfWork.PartnerRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return request;
        }

        public async Task<string> DeletePartner(string email)
        {
            var exist = await PartnerExist(email);
            if(exist == null)
            {
                throw new KeyNotFoundException("Partner does not exist");
            }
            _unitOfWork.PartnerRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return "Success";
        }
    }
}
