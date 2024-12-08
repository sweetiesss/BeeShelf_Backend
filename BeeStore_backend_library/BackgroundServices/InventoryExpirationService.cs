using BeeStore_Repository.Logger;

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
            SetInterval(TimeSpan.FromSeconds(10));
            _unitOfWork = unitOfWork;
        }

        protected override async Task PerformPeriodicTaskAsync(CancellationToken stoppingToken)
        {

            var inventory = await _unitOfWork.InventoryRepo.GetFiltered(x => x.ExpirationDate.HasValue
                                                                          && x.OcopPartnerId.HasValue);


            // Update expired items
            foreach (var item in inventory)
            {
                var timeLeft = (item.ExpirationDate.Value.Date - DateTime.Now.Date).TotalDays;
                if (timeLeft <= 3 && timeLeft > 0)
                {
                    _logger.LogDebug($"Send an email here: {item.Id}");
                }
            }


        }
    }
}
