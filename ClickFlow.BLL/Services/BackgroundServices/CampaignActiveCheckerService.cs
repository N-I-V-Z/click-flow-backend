using ClickFlow.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClickFlow.BLL.Services.BackgroundServices
{
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
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
			return Task.CompletedTask;
		}

		private async void DoWork(object state)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var campaignService = scope.ServiceProvider.GetRequiredService<ICampaignService>();
				await campaignService.UpdateCampaignActiveStatus();
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
}
