using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarDealershipApi.Services;

public class OtpCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OtpCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10); // Clean up every 10 minutes

    public OtpCleanupService(IServiceProvider serviceProvider, ILogger<OtpCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OTP Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var otpService = scope.ServiceProvider.GetRequiredService<IOtpService>();
                    await otpService.CleanupExpiredOtpsAsync();
                }

                _logger.LogDebug("OTP cleanup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OTP cleanup");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("OTP Cleanup Service stopped");
    }
}

