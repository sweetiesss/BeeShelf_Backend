using BeeStore_Repository.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.BackgroundServices
{
    public class LotExpirationService : BaseBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LotExpirationService(
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

            var lots = await _unitOfWork.LotRepo.GetFiltered(x => x.ImportDate.HasValue
                                                                          && x.IsDeleted != true);


            // Update expired items
            foreach (var item in lots)
            {
                var timeLeft = (item.ExpirationDate.Value.Date - DateTime.Now.Date).TotalDays;
                if (timeLeft <= 5 && timeLeft > 0)
                {
                    _logger.LogDebug($"Send an email here: {item.Id}");
                }
                if (timeLeft <= 0)
                {
                    //save the name here
                    item.IsDeleted = true;
                    
                }
            }

                    //send an email afterward with the list of names

        }
    }
}
