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

        public async Task<Pagination<PartnerListDTO>> GetPartnerList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.PartnerRepo.GetAllAsync();
            var result = _mapper.Map<List<PartnerListDTO>>(list);
            
            return await ListPagination<PartnerListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<PartnerUpdateRequest> UpgradeToPartner(PartnerUpdateRequest request)
        {
            var user = await _unitOfWork.UserRepo.SingleOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (user.RoleId != 6)
            {
                throw new ApplicationException(ResponseMessage.UserRoleError);
            }

            
            //if (await _unitOfWork.PartnerRepo.SingleOrDefaultAsync(u => u.UserId == request.UserId) != null)
            //{
            //    throw new DuplicateException("User is already a partner");
            //}
            request.CreateDate = DateTime.Now;
            request.UpdateDate = DateTime.Now;
            var partner = _mapper.Map<Partner>(request);
            await _unitOfWork.PartnerRepo.AddAsync(partner);
            user.RoleId = 4;
            await _unitOfWork.SaveAsync();
            

            return request;
        }

        public async Task<PartnerUpdateRequest> UpdatePartner(PartnerUpdateRequest request)
        {
            var exist = await _unitOfWork.PartnerRepo.SingleOrDefaultAsync(u => u.UserId == request.UserId);
            if(exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
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
