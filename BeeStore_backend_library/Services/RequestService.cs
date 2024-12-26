using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.RequestDTOs;
using BeeStore_Repository.Enums;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeeStore_Repository.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public RequestService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CancelRequest(int id, string? cancellationReason)
        {
            var request = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (request == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (request.Status.Equals(Constants.Status.Draft))
            {
                throw new ApplicationException(ResponseMessage.RequestDraftCancelError);
            }
            if (!request.Status.Equals(Constants.Status.Pending))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            if (cancellationReason != null)
            {
                request.CancellationReason = cancellationReason;
            }
            request.CancelDate = DateTime.Now;
            request.Status = Constants.Status.Canceled;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> SendRequest(int id)
        {
            var request = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id);
            if (request == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!request.Status.Equals(Constants.Status.Draft))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            request.Status = Constants.Status.Pending;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }


        public async Task<string> CreateRequest(RequestType type, bool Send, RequestCreateDTO request)
        {
            var user = await _unitOfWork.OcopPartnerRepo.SingleOrDefaultAsync(u => u.Id == request.OcopPartnerId);
            if (user == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id == request.SendToRoomId, query => query.Include(o => o.Store));
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }
            var userInventory = await _unitOfWork.RoomRepo.AnyAsync(u => u.Id.Equals(inventory.Id)
                                                                           && u.OcopPartner.Id.Equals(user.Id));
            if (userInventory == false)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryPartnerNotMatch);
            }

            int totalProductAmount = 0;
            //gotta throw the export here because it will fuck up the other thing.
            request.RequestType = type.ToString();
            if (request.ExportFromLotId != 0)
            {
                if (request.RequestType == "Import")
                {
                    throw new ApplicationException(ResponseMessage.BadRequest);
                }
                var originalLot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(request.ExportFromLotId), inc => inc.Include(o => o.Room));
                if (originalLot != null)
                {
                    if (originalLot.Room.StoreId == inventory.StoreId)
                    {
                        throw new ApplicationException(ResponseMessage.InventoryFromTheSameWarehouse);
                    }
                    request.Lot.ProductId = (int)originalLot.ProductId;
                    request.Lot.ProductPerLot = originalLot.ProductPerLot;
                    //recalculate here because i'm a moron and I don't want to switch shit around too much
                    totalProductAmount = (int)(request.Lot.ProductPerLot * request.Lot.LotAmount);

                    if (originalLot.TotalProductAmount < totalProductAmount)
                    {
                        throw new ApplicationException(ResponseMessage.ProductNotEnough);
                    }
                    request.Lot.LotNumber = originalLot.LotNumber;
                    request.Lot.Name = originalLot.Name;
                }
                else
                {
                    throw new ApplicationException(ResponseMessage.PackageIdNotFound);
                }
            }
            else
            {
                request.ExportFromLotId = null;
            }


            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.Lot.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }
            //check for cold shit
            if (product.IsCold.Value != inventory.IsCold.Value)
            {
                throw new ApplicationException(ResponseMessage.ProductAndRoomTypeNotMatch);
            }


            var userProduct = await _unitOfWork.ProductRepo.AnyAsync(u => u.Id.Equals(product.Id)
                                                                       && u.OcopPartnerId.Equals(user.Id));
            if (userProduct == false)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductPartnerNotMatch);
            }
            if (totalProductAmount == 0)
            {
                totalProductAmount = (int)(request.Lot.ProductPerLot * request.Lot.LotAmount);
            }

            var totalWeight = inventory.Weight + product.Weight * totalProductAmount;


            if (totalWeight > inventory.MaxWeight)
            {
                throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
            }

            if (Send == true)
            {
                request.Status = Constants.Status.Pending;
            }
            else
            {
                request.Status = Constants.Status.Draft;
            }
            var result = _mapper.Map<Request>(request);
            result.Lot.TotalProductAmount = totalProductAmount;


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
            if (exist.Status != Constants.Status.Draft)
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            _unitOfWork.RequestRepo.SoftDelete(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<List<Request>> ApplyFilterToList(bool? import, RequestStatus? requestStatus, bool descending,
                                                          int? userId = null, int? storeId = null)
        {
            string? filterQuery = requestStatus switch
            {
                RequestStatus.Draft => Constants.Status.Draft,
                RequestStatus.Pending => Constants.Status.Pending,
                RequestStatus.Canceled => Constants.Status.Canceled,
                RequestStatus.Processing => Constants.Status.Processing,
                RequestStatus.Delivered => Constants.Status.Delivered,
                RequestStatus.Completed => Constants.Status.Completed,
                RequestStatus.Failed => Constants.Status.Failed,
                _ => null
            };

            var list = await _unitOfWork.RequestRepo.GetListAsync(
                filter: u => (filterQuery == null || u.Status.Equals(filterQuery))
                             && (userId == null || u.OcopPartnerId.Equals(userId))
                             && (storeId == null || u.SendToRoom.StoreId.Equals(storeId) || u.ExportFromLot.Room.StoreId.Equals(storeId))
                             && (import == null ||
        (import == true && u.RequestType.Equals("Import")) ||
        (import == false && u.RequestType.Equals("Export")))
                             && u.IsDeleted.Equals(false),
                includes: u => u.Include(o => o.SendToRoom).ThenInclude(o => o.Store)
                                .Include(o => o.ExportFromLot).ThenInclude(o => o.Room).ThenInclude(o => o.Store)
                                .Include(o => o.OcopPartner)
                                .Include(o => o.Lot).ThenInclude(o => o.Product),
                sortBy: Constants.SortCriteria.CreateDate,
                descending: descending,
                searchTerm: null,
                searchProperties: null
                );
            return list;
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(bool? import, RequestStatus? status, bool descending, int storeId, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(import, status, descending, null, storeId);
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }

        public async Task<Pagination<RequestListDTO>> GetRequestList(int userId, bool? import, RequestStatus? status, bool descending, int pageIndex, int pageSize)
        {
            var list = await ApplyFilterToList(import, status, descending, userId);
            var result = _mapper.Map<List<RequestListDTO>>(list);
            return await ListPagination<RequestListDTO>.PaginateList(result, pageIndex, pageSize);
        }


        public async Task<string> UpdateRequest(int id, RequestCreateDTO request)
        {
            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id, include => include.Include(o => o.Lot));
            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (!exist.Status.Equals(Constants.Status.Draft))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }
            var user = await _unitOfWork.OcopPartnerRepo.AnyAsync(u => u.Id == request.OcopPartnerId);
            if (user == false)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            var inventory = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id == request.SendToRoomId, include => include.Include(o => o.Store));
            if (inventory == null)
            {
                throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
            }

            var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id == request.Lot.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException(ResponseMessage.ProductIdNotFound);
            }

            //check for cold shit
            if (product.IsCold.Value != inventory.IsCold.Value)
            {
                throw new ApplicationException(ResponseMessage.ProductAndRoomTypeNotMatch);
            }

            if (exist.Lot.RoomId != request.SendToRoomId)
            {
                exist.Lot.RoomId = request.SendToRoomId;
            }
            exist.Lot.ProductId = request.Lot.ProductId;
            exist.Lot.LotAmount = request.Lot.LotAmount;
            exist.Lot.LotNumber = request.Lot.LotNumber;
            exist.Lot.Name = request.Lot.Name;
            exist.Lot.ProductPerLot = request.Lot.ProductPerLot;
            exist.Lot.TotalProductAmount = request.Lot.ProductPerLot * request.Lot.LotAmount;

            exist.SendToRoomId = request.SendToRoomId;
            exist.Description = request.Description;
            _unitOfWork.RequestRepo.Update(exist);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }


        public async Task<string> UpdateRequestStatus(int id, RequestStatus status, int? staffId)
        {

            var exist = await _unitOfWork.RequestRepo.SingleOrDefaultAsync(u => u.Id == id, includes => includes.Include(o => o.Lot).ThenInclude(o => o.Room)
                                                                                                                .Include(o => o.ExportFromLot).ThenInclude(o => o.Room)
                                                                                                                .Include(o => o.SendToRoomId));

                var staff = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id == staffId, query => query.Include(o => o.StoreStaffs));
                if (staff == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
                }

            if (exist == null)
            {
                throw new KeyNotFoundException(ResponseMessage.RequestIdNotFound);
            }
            if (exist.Status.Equals(Constants.Status.Completed)
                || exist.Status.Equals(Constants.Status.Failed)
                || exist.Status.Equals(Constants.Status.Canceled))
            {
                throw new ApplicationException(ResponseMessage.RequestStatusError);
            }

            string? requestStatus = status switch
            {
                RequestStatus.Draft => string.Empty,
                RequestStatus.Pending => string.Empty,
                RequestStatus.Canceled => string.Empty,
                RequestStatus.Processing => await UpdateProcessingStatusRequest(status.ToString(), staff, exist),
                RequestStatus.Delivered => await UpdateDeliveredStatusRequest(status.ToString(), staff, exist),
                RequestStatus.Completed => await UpdateCompletedStatusRequest(status.ToString(), staff, exist),
                RequestStatus.Failed => await UpdateFailedStatusRequest(status.ToString(), staff, exist),
                RequestStatus.Returned => await UpdateReturnedStatusRequest(status.ToString(), staff, exist),
                _ => string.Empty
            };
            if (requestStatus.Equals(string.Empty))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            //Pending to processing (both import and export)
            //await UpdateProcessingStatusRequest(requestStatus, staff, exist);

            //Processing to Delivered
            //await UpdateDeliveredStatusRequest(requestStatus, staff, exist);

            //Delivered to Completed
            //await UpdateCompletedStatusRequest(requestStatus, staff, exist);

            //From processing/delivered to Failed
            //await UpdateFailedStatusRequest(requestStatus, staff, exist);

            //From Delivered to Returned    
            //await UpdateReturnedStatusRequest(requestStatus, staff, exist);
            return ResponseMessage.Success;

        }
        private async Task<string> UpdateProcessingStatusRequest(string requestStatus,Employee staff, Request exist)
        {
            //Pending to processing (both import and export)
            if (requestStatus.Equals(Constants.Status.Processing))
            {
                if (!exist.Status.Equals(Constants.Status.Pending))
                {
                    throw new ApplicationException(ResponseMessage.BadRequest);
                }

                if (exist.RequestType == "Export")
                {
                    if (exist.ExportFromLot.Room.StoreId != staff.StoreStaffs.First().StoreId)
                    {
                        throw new ApplicationException(ResponseMessage.StaffCantProcessedExportOrderFromAnotherWarehouse);
                    }
                }

                var lot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.LotId), query => query.Include(o => o.Product));
                if (lot == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                }
                var inventory = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.SendToRoomId));
                if (inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }

                if (exist.RequestType == "Import")
                {
                    //do nothing, just change the status and move on
                }
                else //if export do these things
                {
                    var lotWeight = lot.Product.Weight * lot.TotalProductAmount;
                    var totalWeight = inventory.Weight + lotWeight;
                    if (totalWeight > inventory.MaxWeight)
                    {
                        exist.Status = Constants.Status.Failed;
                        exist.CancellationReason = ResponseMessage.InventoryOverWeightError;
                        await _unitOfWork.SaveAsync();
                        throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
                    }
                    var originalLot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.ExportFromLotId), includes => includes.Include(o => o.Room));
                    if (originalLot != null)
                    {
                        var totalProductAmount = exist.Lot.ProductPerLot * exist.Lot.LotAmount;
                        if (totalProductAmount > originalLot.TotalProductAmount)
                        {
                            exist.Status = Constants.Status.Failed;
                            exist.CancellationReason = ResponseMessage.ProductNotEnough;
                            await _unitOfWork.SaveAsync();
                            throw new ApplicationException(ResponseMessage.ProductNotEnough);
                        }
                        originalLot.TotalProductAmount -= totalProductAmount;
                        originalLot.LotAmount -= exist.Lot.LotAmount;


                        inventory.Weight = totalWeight;
                        originalLot.Room.Weight -= lotWeight;
                    }
                }
                exist.ApporveDate = DateTime.Now;
            }
            exist.Status = requestStatus;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
        
        private async Task<string> UpdateDeliveredStatusRequest(string requestStatus, Employee staff, Request exist)
        {
            if (requestStatus.Equals(Constants.Status.Delivered))
            {
                if (exist.Status != Constants.Status.Processing)
                {
                    throw new ApplicationException(ResponseMessage.RequestHasNotBeenProcessed);
                }
                if (exist.RequestType == "Export")
                {
                    if (exist.SendToRoom.StoreId != staff.StoreShippers.First().StoreId)
                    {
                        throw new ApplicationException(ResponseMessage.StaffCantProcessedExportOrderFromAnotherWarehouse);
                    }
                }
                exist.DeliverDate = DateTime.Now;
            }
            exist.Status = requestStatus;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    
        private async Task<string> UpdateCompletedStatusRequest(string requestStatus, Employee staff, Request exist)
        {
            if (requestStatus == Constants.Status.Completed)
            {
                if (exist.Status != Constants.Status.Delivered)
                {
                    throw new ApplicationException(ResponseMessage.BadRequest);
                }
                var lot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.LotId));
                if (lot == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                }
                var inventory = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.SendToRoomId));
                if (inventory == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                }
                var product = await _unitOfWork.ProductRepo.SingleOrDefaultAsync(u => u.Id.Equals(lot.ProductId));
                if (exist.RequestType == "Import")
                {
                    var totalWeight = inventory.Weight + (product.Weight * lot.TotalProductAmount);
                    if (totalWeight > inventory.MaxWeight)
                    {
                        exist.Status = Constants.Status.Failed;
                        exist.CancellationReason = ResponseMessage.InventoryOverWeightError;
                        await _unitOfWork.SaveAsync();
                        throw new ApplicationException(ResponseMessage.InventoryOverWeightError);
                    }
                    lot.ImportDate = DateTime.Now;
                    lot.RoomId = exist.SendToRoomId;
                    var productcategory = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(product.ProductCategoryId));
                    lot.ExpirationDate = DateTime.Now.AddDays(productcategory!.ExpireIn!.Value);

                    inventory.Weight = totalWeight;
                }
                else
                {
                    if (exist.SendToRoom.StoreId != staff.StoreStaffs.First().StoreId)
                    {
                        throw new ApplicationException(ResponseMessage.StaffCantProcessedExportOrderFromAnotherWarehouse);
                    }
                    var originalLot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.ExportFromLotId), includes => includes.Include(o => o.Room));
                    originalLot.ExportDate = DateTime.Now;
                    lot.ImportDate = DateTime.Now;
                    lot.RoomId = exist.SendToRoomId;
                    var productcategory = await _unitOfWork.ProductCategoryRepo.SingleOrDefaultAsync(u => u.Id.Equals(lot.Product.ProductCategoryId));
                    lot.ExpirationDate = DateTime.Now.AddDays(productcategory!.ExpireIn!.Value);
                }
            }
            exist.Status = requestStatus;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        private async Task<string> UpdateFailedStatusRequest(string requestStatus, Employee staff, Request exist)
        {
            if (requestStatus.Equals(Constants.Status.Failed))
            {
                if (exist.Status != Constants.Status.Processing)
                {
                    if (exist.Status != Constants.Status.Delivered)
                    {
                        throw new ApplicationException(ResponseMessage.RequestHasNotBeenProcessed);
                    }
                    if (exist.RequestType == "Export")
                    {
                        if (exist.SendToRoom.StoreId != staff.StoreStaffs.First().StoreId)
                        {
                            throw new ApplicationException(ResponseMessage.StaffCantProcessedExportOrderFromAnotherWarehouse);
                        }
                    }
                    var lot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.LotId), query => query.Include(o => o.Product));
                    if (lot == null)
                    {
                        throw new KeyNotFoundException(ResponseMessage.PackageIdNotFound);
                    }
                    var inventory = await _unitOfWork.RoomRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.SendToRoomId));
                    if (inventory == null)
                    {
                        throw new KeyNotFoundException(ResponseMessage.InventoryIdNotFound);
                    }
                    var lotWeight = lot.Product.Weight * lot.TotalProductAmount;
                    var originalLot = await _unitOfWork.LotRepo.SingleOrDefaultAsync(u => u.Id.Equals(exist.ExportFromLotId), includes => includes.Include(o => o.Room));
                    if (originalLot != null)
                    {
                        var totalProductAmount = exist.Lot.ProductPerLot * exist.Lot.LotAmount;

                        originalLot.TotalProductAmount += totalProductAmount;
                        originalLot.LotAmount += exist.Lot.LotAmount;


                        inventory.Weight -= lotWeight;
                        originalLot.Room.Weight += lotWeight;
                        lot.IsDeleted = true;
                    }

                    exist.CancelDate = DateTime.Now;
                }
            }
            exist.Status = requestStatus;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
        
        private async Task<string> UpdateReturnedStatusRequest(string requestStatus, Employee staff, Request exist)
        {
            if (requestStatus.Equals(Constants.Status.Returned))
            {
                if (exist.Status != Constants.Status.Delivered)
                {
                    throw new ApplicationException(ResponseMessage.RequestHasNotBeenProcessed);
                }

                exist.CancelDate = DateTime.Now;
            }
            exist.Status = requestStatus;
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

    }
    
}