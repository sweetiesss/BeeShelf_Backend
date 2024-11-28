using AutoMapper;
using BeeStore_Repository.Logger;
using BeeStore_Repository.Models;
using BeeStore_Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Services
{
    public class BatchDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public BatchDeliveryService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> createBatchDelivery(int batchId, int numberOfTrip, DateTime startDate, List<Order> orders)
        {
            // Assuming every data always passed correctly since this will not be used by any Controller nor FE
            BatchDelivery batchDelivery = new BatchDelivery();
            batchDelivery.BatchId = batchId;
            batchDelivery.Orders = orders;
            batchDelivery.NumberOfTrips = numberOfTrip;
            batchDelivery.DeliveryStartDate = startDate;

            await _unitOfWork.BatchDeliveryRepo.AddAsync(batchDelivery);
            await _unitOfWork.SaveAsync();
            return ResponseMessage.Success;
        }
    }
}
