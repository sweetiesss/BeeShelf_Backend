using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace BeeStore_Repository.Services
{
    public class RequestService : IRequestService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public RequestService(UnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateRequest(RequestCreateDTO request)
        {
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == request.OcopPartnerId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }

            var package = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(request.LotId),
                                                                                query => query.Include(o => o.Product)
                                                                                .ThenInclude(item => item.ProductCategory));
            if (package == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            int totalWeight = 1;
            //var totalWeight = inventory.Weight + (package.Product.Weight * package.ProductAmount);
            if (totalWeight > inventory.MaxWeight)
            {
                throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
            }
            request.Status = Constants.Status.Pending;
            var result = _mapper.Map<Request>(request);
            await _unitOfWork.RequestRepo.AddAsync(result);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteRequest(int id)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            _unitOfWork.RequestRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.RequestRepo.GetAllAsync();
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(int userId, int pageIndex, int pageSize)
        {
            var list = await _unitOfWork.RequestRepo.GetFiltered(u => u.OcopPartnerId.Equals(userId));
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<string> UpdateRequest(int id, RequestCreateDTO request)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            var user = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == request.OcopPartnerId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id == request.SendToInventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var package = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id == request.LotId);
            if (package == null)
            {
                throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
            }
            if (!exist.Status.Equals(Constants.Status.Pending, StringComparison.OrdinalIgnoreCase))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            exist.SendToInventoryId = request.SendToInventoryId;
            exist.Description = request.Description;
            exist.LotId = request.LotId;
            exist.RequestType = request.RequestType;
            _unitOfWork.RequestRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        //this entire function is fucked, fix it later
        public async Task<string> UpdateRequestStatus(int id, int statusId)
        {
            string status = null;
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!exist.Status.Equals(Constants.Status.Pending))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            if (statusId == 1)
            {
                status = Constants.Status.Approved;

                var package = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.LotId));
                if (package == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                }
                var inventory = await _unitOfWork.InventoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.SendToInventory));
                if (inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
                var totalWeight = inventory.MaxWeight + (package.Product.Weight * package.ProductAmount);
                if (totalWeight > inventory.MaxWeight)
                {
                    throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
                }

                package.InventoryId = exist.SendToInventoryId;
                package.ExpirationDate = DateTime.Now.AddDays(package.Product.ProductCategory!.ExpireIn!.Value);

                inventory.MaxWeight = 1;

            }
            if (statusId == 2)
            {
                status = Constants.Status.Reject;
            }
            exist.Status = status;
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<RequestListDTO>(exist);
            return ResponseMessage.Success;
        }
    }
}
