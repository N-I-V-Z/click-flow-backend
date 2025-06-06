using ClickFlow.BLL.Helpers.Config;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.Extensions.Options;
using PusherServer;

namespace ClickFlow.BLL.Services.Implements
{
    public class PusherService : IPusherService
    {
        private readonly Pusher _pusher;

        public PusherService(IOptions<PusherConfiguration> options)
        {
            var config = options.Value;
            _pusher = new Pusher(
                config.AppId,
                config.Key,
                config.Secret,
                new PusherOptions
                {
                    Cluster = config.Cluster,
                    Encrypted = true
                });
        }

        public async Task TriggerMessageAsync(string channel, string eventName, object data)
        {
            await _pusher.TriggerAsync(channel, eventName, data);
        }

        public Pusher GetPusherClient() => _pusher;

    }
}
