using BeeStore_Repository.Logger;
using Microsoft.Extensions.Hosting;


namespace BeeStore_Repository.BackgroundServices
{
        public abstract class BaseBackgroundService : BackgroundService
        {
            protected readonly IServiceProvider _serviceProvider;
            protected readonly ILoggerManager _logger;

            
            protected TimeSpan DefaultInterval { get; set; } = TimeSpan.FromSeconds(10);

            protected BaseBackgroundService(
                IServiceProvider serviceProvider,
                ILoggerManager logger)
            {
                _serviceProvider = serviceProvider;
                _logger = logger;
            }

            
            protected abstract Task PerformPeriodicTaskAsync(CancellationToken stoppingToken);

            
            protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInfo($"Starting periodic task for {GetType().Name} at {DateTime.Now}");

                        await PerformPeriodicTaskAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"EXCEPTION: {ex}, Error in background service {GetType().Name}");
                    }

                    // Wait for the next interval
                    await Task.Delay(DefaultInterval, stoppingToken);
                }
            }

            
            protected void SetInterval(TimeSpan interval)
            {
                DefaultInterval = interval;
            }
        }
    }
