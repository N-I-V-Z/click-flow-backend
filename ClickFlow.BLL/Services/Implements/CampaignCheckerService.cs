using ClickFlow.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class CampaignCheckerService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public CampaignCheckerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Khởi tạo Timer với chu kỳ 5 phút
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var campaignService = scope.ServiceProvider.GetRequiredService<ICampaignService>();
            await campaignService.CheckAndStopExpiredCampaigns();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Dừng Timer khi service dừng
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}