using BeeStore_Repository.Logger;
using Microsoft.EntityFrameworkCore;

namespace BeeStore_Repository.BackgroundServices
{
    public class InventoryExpirationService : BaseBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryExpirationService(
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

            var inventory = await _unitOfWork.InventoryRepo.GetQueryable(x => x.Include(o => o.Lots)
                                                                                .Where(u => u.ExpirationDate.HasValue
                                                                                    && u.OcopPartnerId.HasValue));

            inventory = inventory.ToList();
            // Update expired items
            foreach (var item in inventory)
            {
                var timeLeft = (item.ExpirationDate.Value.Date - DateTime.Now.Date).TotalDays;
                if (timeLeft <= 3 && timeLeft > 0)
                {
                    _logger.LogDebug($"Send an email here: {item.Id}");
                }
                if(timeLeft <= 0)
                {
                    foreach(var lot in item.Lots)
                    {
                        //add the lotname to a list so you can send an email later.
                        lot.IsDeleted = true;
                    }
                    //save inventory ocoppartneremail first before null
                    item.OcopPartnerId = null;
                }
            }

                    //send an email afterward

        }
    }
}
