using ClickFlow.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClickFlow.BLL.Services.BackgroundServices
{
	public class TrafficToClosedTrafficService : IHostedService, IDisposable
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<TrafficToClosedTrafficService> _logger;
		private Timer _timer;

		public TrafficToClosedTrafficService(IServiceScopeFactory scopeFactory, ILogger<TrafficToClosedTrafficService> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("TrafficToClosedTrafficService started.");

			_timer = new Timer(_ =>
			{
				DoWork().GetAwaiter().GetResult();
			}, null, TimeSpan.Zero, TimeSpan.FromDays(1));

			return Task.CompletedTask;
		}

		private async Task DoWork()
		{
			_logger.LogInformation("Processing traffic data...");

			try
			{
				// ✅ Luôn tạo scope mới khi sử dụng service Scoped
				using (var scope = _scopeFactory.CreateScope())
				{
					var trafficService = scope.ServiceProvider.GetRequiredService<ITrafficService>();
					await trafficService.TransferTrafficToClosedTraffic();
					_logger.LogInformation("Traffic processing completed.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while processing traffic.");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("TrafficToClosedTrafficService stopping...");
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}

}
