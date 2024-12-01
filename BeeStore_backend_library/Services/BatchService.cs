using Amazon.S3.Model;
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
using MySqlX.XDevAPI.Common;
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

        //Must assgin only 1 shipper when Create Batch - Assigned Shipper cannot be changed
        public async Task<string> CreateBatch(BatchCreateDTO request)
        {
            var result = _mapper.Map<Batch>(request);
            result.Status = Constants.Status.Pending;
            await _unitOfWork.BatchRepo.AddAsync(result);

            DateTime now = DateTime.Now;
            DateTime nextHour = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);

            //Sort Order by hand (fix this if you know how)
            var orderList = new List<Order>();
            foreach(var o in request.Orders)
            {
                var order = await _unitOfWork.OrderRepo.SingleOrDefaultAsync(a => a.Id.Equals(o.Id));
                if (order == null)
                    throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);

                if (!order.Status.Equals(Constants.Status.Processing))
                    throw new ApplicationException(ResponseMessage.BatchAssignedOrder);

                if (order.BatchDeliveryId != null)
                    throw new ApplicationException(ResponseMessage.OrderBatchError);

                if(!order.DeliveryZoneId.Equals(request.DeliveryZoneId))
                    throw new ApplicationException(ResponseMessage.DeliveryZoneOrderNotMatch);

                orderList.Add(order);
            }
            orderList = orderList.OrderBy(o => o.TotalWeight).ToList();

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

                var warehouseShipper = await _unitOfWork.WarehouseShipperRepo.SingleOrDefaultAsync(u => u.EmployeeId.Equals(shipper.Id));

                if (!warehouseShipper.DeliveryZoneId.Equals(request.DeliveryZoneId))
                {
                    throw new ApplicationException(ResponseMessage.DeliveryZoneShipperNotMatch);
                }

                if (shipper.Role.RoleName != Constants.RoleName.Shipper)
                {
                    throw new ApplicationException(ResponseMessage.UserRoleNotShipperError);
                }
                //I was going to check here but wouldn't a cold warehouse have all cold vehicles? 
                // why would you have any other vehicle in an all cold product warehouse?
                //if(shipper.Vehicles.FirstOrDefault(u => u.AssignedDriverId.Equals(shipper.Id)).IsCold 
                //    != orderList[0].OrderDetails.First().Lot.Product.IsCold)
                //{
                //    throw new ApplicationException(ResponseMessage.ShipperDeliverColdProduct);
                //}

                result.DeliverBy = shipper.Id;
                var vehicle = shipper.Vehicles.FirstOrDefault(u => u.AssignedDriverId.Equals(shipper.Id) && u.IsDeleted.Equals(false));
                //Check Order -> Create Batch Delivery
                decimal? currentWeight = 0;
                var cap = vehicle.Capacity;
                List<Order> tempOrder = new List<Order>();
                for(int i = 0; i < orderList.Count; i++) {
                    currentWeight += orderList[i].TotalWeight;
                    if (currentWeight > cap)
                    {
                        int trips = 1;
                        if (tempOrder.Count == 0)
                        {
                            trips = (int)(currentWeight / cap);
                            if (trips * cap != currentWeight) trips++;
                            tempOrder.Add(orderList[i]);
                        }

                        BatchDelivery batchDelivery = new BatchDelivery
                        {
                            NumberOfTrips = trips,
                            DeliveryStartDate = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond)
                        };


                        // can optimize this - Change batchDeliveryId of the Order
                        for (int j = 0; j < tempOrder.Count; j++)
                        {
                            tempOrder[j].BatchDelivery = batchDelivery;
                            _unitOfWork.OrderRepo.Update(tempOrder[j]);
                        }

                        result.BatchDeliveries.Add(batchDelivery);
                        tempOrder.Clear();
                        currentWeight = 0;
                        if(trips == 1) i--;
                    }else tempOrder.Add(orderList[i]);
                }
                
                if (tempOrder.Count > 0) {
                    BatchDelivery batchDelivery = new BatchDelivery
                    {
                        BatchId = result.Id,
                        Orders = tempOrder,
                        NumberOfTrips = 1,
                        DeliveryStartDate = now.AddHours(1).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond)
                    };
                    for (int j = 0; j < tempOrder.Count; j++)
                    {
                        tempOrder[j].BatchDelivery = batchDelivery;
                        tempOrder[j].DeliverStartDate = batchDelivery.DeliveryStartDate;
                        _unitOfWork.OrderRepo.Update(tempOrder[j]);
                    }
                    await _unitOfWork.BatchDeliveryRepo.AddAsync(batchDelivery);
                    result.BatchDeliveries.Add(batchDelivery);
                }
            }
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        // NOT USED
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


            if(request.ShipperId != null)
                batch.DeliverBy = request.ShipperId;
            
            if(request.Name != null)
                batch.Name = request.Name;


            //remember to fix these
            //foreach (var o in batch.Orders)
            //{
            //    o.BatchId = null;
            //}
            //    await _unitOfWork.SaveAsync();

            foreach (var o in request.Orders)
            {
                if (await _unitOfWork.OrderRepo.AnyAsync(u => u.Id == o.Id && u.IsDeleted == false) == false)
                {
                    throw new KeyNotFoundException(ResponseMessage.OrderIdNotFound);
                }
                //remember to fix these
                //if (await _unitOfWork.OrderRepo.AnyAsync(u => u.Id.Equals(o.Id) && u.BatchId != null))
                //{
                //    throw new ApplicationException(ResponseMessage.OrderBatchError);
                //}
                o.BatchId = batch.Id;
            }

            batch = _mapper.Map<Batch>(request);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Unsupported;
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
            foreach (var o in batch.BatchDeliveries)
            {
                o.BatchId = null;
                o.IsDeleted = true;
                var list = o.Orders.Where(u => u.BatchDeliveryId.Equals(o.Id)).ToList();
                foreach(var i in list)
                {
                    i.BatchDeliveryId = null;
                }
            }
            
            _unitOfWork.BatchRepo.SoftDelete(batch);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }

        // NOT USED
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
                //DeliverBy = shipper.Id
            });
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Unsupported;
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
                case BatchFilter.WarehouseId: filterExpression = u => u.DeliveryZone.WarehouseShippers.Any(u => u.WarehouseId.Equals(Int32.Parse(filterQuery!))); break;
                case BatchFilter.DeliveryZoneId: filterExpression = u => u.DeliveryZoneId.Equals(Int32.Parse(filterQuery!)); break;
                case BatchFilter.Status: filterExpression = u => u.Status.Equals(filterQuery!, StringComparison.OrdinalIgnoreCase); break;
                default: filterExpression = null; break;
            }

            var list = await _unitOfWork.BatchRepo.GetListAsync(
                filter: filterExpression!,
                includes: u => u.Include(o => o.BatchDeliveries).Include(o => o.DeliveryZone),
                sortBy: null,
                descending: false,
                searchTerm: search,
                searchProperties: new Expression<Func<Batch, string>>[] { o => o.Name }
                );

            var result = _mapper.Map<List<BatchListDTO>>(list);
            return (await ListPagination<BatchListDTO>.PaginateList(result, pageIndex, pageSize));
        }

        public async Task<Pagination<BatchListDTO>> GetShipperBatchList(int shipperId, BatchFilter? filterBy, string? filterQuery, int pageIndex, int pageSize)
        {
            if ((!string.IsNullOrEmpty(filterQuery) && filterBy == null)
                 || (string.IsNullOrEmpty(filterQuery) && filterBy != null))
            {
                throw new BadHttpRequestException(ResponseMessage.BadRequest);
            }

            Expression<Func<Batch, bool>> filterExpression = null;
            switch (filterBy)
            {
                case BatchFilter.DeliveryZoneId: filterExpression = u => u.DeliveryZoneId.Equals(Int32.Parse(filterQuery!)) && u.DeliverBy.Equals(shipperId); break;
                case BatchFilter.Status: filterExpression = u => u.Status.Equals(filterQuery!, StringComparison.OrdinalIgnoreCase) && u.DeliverBy.Equals(shipperId); break;
                default: filterExpression = u => u.DeliverBy.Equals(shipperId); break;
            }

            var list = await _unitOfWork.BatchRepo.GetListAsync(
               filter: filterExpression!,
               includes: u => u.Include(o => o.BatchDeliveries).ThenInclude(o => o.Orders),
               sortBy: null,
               descending: false,
               searchTerm: null,
               searchProperties: null
               );

            var result = _mapper.Map<List<BatchListDTO>>(list);
            return (await ListPagination<BatchListDTO>.PaginateList(result, pageIndex, pageSize));
        }
    }
}
