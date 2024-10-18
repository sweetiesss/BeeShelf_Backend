using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.PartnerDTOs;
using BeeStore_Repository.Enums.SortBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;

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

        public async Task<Pagination<PartnerListDTO>> GetPartnerList(SortBy sortby, bool descending, int pageIndex, int pageSize)
        {
            string sortCriteria = sortby.ToString();

            var list = await _unitOfWork.PartnerRepo.GetListAsync(
                filter: null,
                sortBy: sortCriteria,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
                
            var result = _mapper.Map<List<PartnerListDTO>>(list);

            return await ListPagination<PartnerListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<string> UpgradeToPartner(PartnerUpdateRequest request)
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

            request.CreateDate = DateTime.Now;
            request.UpdateDate = DateTime.Now;
            var partner = _mapper.Map<Partner>(request);
            await _unitOfWork.PartnerRepo.AddAsync(partner);
            user.RoleId = 4;
            await _unitOfWork.SaveAsync();


            return ResponseMessage.Success;
        }

        public async Task<string> UpdatePartner(PartnerUpdateRequest request)
        {
            var exist = await _unitOfWork.PartnerRepo.SingleOrDefaultAsync(u => u.UserId == request.UserId);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            exist.UpdateDate = DateTime.Now;
            if (!String.IsNullOrEmpty(request.BankAccountNumber) &&
                !request.BankAccountNumber.Equals(Constants.DefaultString.String))
            {
                exist.BankAccountNumber = request.BankAccountNumber;
            }
            if (!String.IsNullOrEmpty(request.CitizenIdentificationNumber) &&
                !request.CitizenIdentificationNumber.Equals(Constants.DefaultString.String))
            {
                exist.CitizenIdentificationNumber = request.CitizenIdentificationNumber;
            }
            if (!String.IsNullOrEmpty(request.BankName) &&
                !request.BankName.Equals(Constants.DefaultString.String))
            {
                exist.BankName = request.BankName;
            }

            _unitOfWork.PartnerRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
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
