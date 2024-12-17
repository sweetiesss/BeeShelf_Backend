using BeeStore_Repository.Logger;
using BeeStore_Repository.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.BackgroundServices
{
    public class RequestStatusChangeService : BaseBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RequestStatusChangeService(
            IServiceProvider serviceProvider,
            ILoggerManager logger,
            IUnitOfWork unitOfWork)
            : base(serviceProvider, logger)
        {
            SetInterval(TimeSpan.FromHours(12));
            _unitOfWork = unitOfWork;
        }

        protected override async Task PerformPeriodicTaskAsync(CancellationToken stoppingToken)
        {

            var requests = await _unitOfWork.RequestRepo.GetQueryable(x => x.Include(o => o.Lot)
                                                                                .Where(u => u.Status == Constants.Status.Processing
                                                                                    && u.IsDeleted.Equals(false)));

            requests = requests.ToList();
            // Update expired items
            foreach (var item in requests)
            {
                var time = (item.ApporveDate.Value.Date - DateTime.Now.Date).TotalDays;
                if (time >= 6)
                {
                    if(item.Status != Constants.Status.Failed)
                    {
                        item.Status = Constants.Status.Failed;
                        item.CancellationReason = "Deliver window expired.";
                        item.CancelDate = DateTime.Now;
                        item.Lot.IsDeleted = true;
                    }
                }
                
            }
            await _unitOfWork.SaveAsync();

            //send an email afterward (maybe)

        }
    }
}
