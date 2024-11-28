using AutoMapper;
using BeeStore_Repository.DTO;
using BeeStore_Repository.DTO.Batch;
using BeeStore_Repository.Enums.FilterBy;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Services.Interfaces;
using BeeStore_Repository.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BeeStore_Repository.Services
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public BatchService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<string> CreateBatch(BatchCreateDTO request)
        {
            //I move the save async to the bottom so hopefully it will fix the issue of batch still added to database
            //if an error is caught in the later check
            decimal? totalWeight = 0;
            var result = _mapper.Map<Batch>(request);
            result.Status = Constants.Status.Pending;
            await _unitOfWork.BatchRepo.AddAsync(result);

            DateTime now = DateTime.Now;
            DateTime nextHour = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            
            //Check Order
            foreach (var o in request.Orders)
            {
                var order = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(u => u.Id == o.Id);
                if(order == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
                }
                if(order.BatchId != null)
                {
                    throw new ApplicationException(ResponseMessage.OrderBatchError);

                }
                if (order.Status != Constants.Status.Processing)
                {
                    throw new ApplicationException(ResponseMessage.BatchAssignedOrder);
                }
                totalWeight += order.TotalWeight;
                order.BatchId = result.Id;
                order.DeliverStartDate = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            }
            //check shipper
            if (request.ShipperId != 0)
            {
                var shipper = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(request.ShipperId),
                                                                                 query => query.Include(o => o.Role)
                                                                                               .Include(o => o.Vehicles));
                if (shipper == null)
                {
                    throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
                }
                if (shipper.Role.RoleName != Constants.RoleName.Shipper)
                {
                    throw new ApplicationException(ResponseMessage.UserRoleNotShipperError);
                }

                //this just check for this batch of orders only, it should check the total weight
                //of all order that is currently being shipped but if I do that it will be slow
                //as shit so you can do whatever you want.
                if (shipper.Vehicles.FirstOrDefault(u => u.AssignedDriverId.Equals(shipper.Id)).Capacity < totalWeight)
                {
                    throw new ApplicationException(ResponseMessage.ShipperBatchOrdersOverweight);
                }

                result.BatchDeliveries.Add(
                    new BatchDelivery
                    {
                        NumberOfTrips = 1,
                        DeliveryStartDate = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond),
                        BatchId = result.Id,
                        DeliverBy = request.ShipperId
                    }
                );
            }
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> UpdateBatch(int id, BatchCreateDTO request)
        {
            var batch = await _unitOfWork.BatchRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (batch == null)
            {
                throw new KeyNotFoundException(ResponseMessage.BatchIdNotFound);
            }
            if (batch.Status != Constants.Status.Pending)
            {
                throw new ApplicationException(ResponseMessage.BatchStatusNotPending);
            }
            foreach (var o in batch.Orders)
            {
                o.BatchId = null;
            }
                await _unitOfWork.SaveAsync();

            foreach (var o in request.Orders)
            {
                if (await _unitOfWork.OrderRepo.AnyAsync(u => u.Id == o.Id && u.IsDeleted == false) == false)
                {
                    throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
                }
                if (await _unitOfWork.OrderRepo.AnyAsync(u => u.Id.Equals(o.Id) && u.BatchId != null))
                {
                    throw new ApplicationException(ResponseMessage.OrderBatchError);
                }
                o.BatchId = batch.Id;
            }
            batch = _mapper.Map<Batch>(request);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> DeleteBatch(int id)
        {
            var batch = await _unitOfWork.BatchRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (batch == null)
            {
                throw new KeyNotFoundException(ResponseMessage.BatchIdNotFound);
            }
            if (batch.Status != Constants.Status.Pending)
            {
                throw new ApplicationException(ResponseMessage.BatchStatusNotPending);
            }
            foreach (var o in batch.Orders)
            {
                o.BatchId = null;
                await _unitOfWork.SaveAsync();
            }
            _unitOfWork.BatchRepo.SoftDelete(batch);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<string> AssignBatch(int id, int shipperId)
        {
            var batch = await _unitOfWork.BatchRepo.SingleOrDefaultAsync(u => u.Id.Equals(id));
            if (batch == null)
            {
                throw new KeyNotFoundException(ResponseMessage.BatchIdNotFound);
            }
            var shipper = await _unitOfWork.EmployeeRepo.SingleOrDefaultAsync(u => u.Id.Equals(shipperId));
            if (shipper == null)
            {
                throw new KeyNotFoundException(ResponseMessage.UserIdNotFound);
            }
            if (shipper.RoleId != 4)
            {
                throw new ApplicationException(ResponseMessage.UserRoleNotShipperError);
            }
            DateTime now = DateTime.Now;
            DateTime nextHour = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            batch.BatchDeliveries.Add(new BatchDelivery
            {
                NumberOfTrips = 1,
                DeliveryStartDate = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond),
                BatchId = batch.Id,
                DeliverBy = shipper.Id
            });
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        public async Task<Pagination<BatchListDTO>> GetBatchList(string search, BatchFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            Expression<Func<Batch, bool>> filterExpression = null;
            switch (filterBy)
            {
                case BatchFilter.DeliveryZoneId: filterExpression = u => u.DeliveryZoneId.Equals(Int32.Parse(filterQuery!)); break;
                case BatchFilter.Status: filterExpression = u => u.Status.Equals(filterQuery!, StringComparison.OrdinalIgnoreCase); break;
                default: filterExpression = null; break;
            }

            var list = await _unitOfWork.BatchRepo.GetListAsync(
                filter: filterExpression!,
                includes: u => u.Include(o => o.BatchDeliveries),
                sortBy: null,
                descending: false,
                searchTerm: search,
                searchProperties: new Expression<Func<Batch, string>>[] { o => o.Name }
                );

            var result = _mapper.Map<List<BatchListDTO>>(list);
            return (await ListPagination<BatchListDTO>.PaginateList(result, pageIndex, pageSize));
        }

    }
}
