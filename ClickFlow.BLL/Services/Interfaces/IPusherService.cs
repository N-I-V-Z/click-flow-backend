using PusherServer;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IPusherService
	{
		Task TriggerMessageAsync(string channel, string eventName, object data);
		Pusher GetPusherClient();

	}
}
