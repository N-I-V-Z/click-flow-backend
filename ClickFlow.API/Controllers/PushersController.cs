using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushersController : BaseAPIController
    {
        private readonly IPusherService _pusherService;
        private readonly IConversationService _conversationService;

        public PushersController(IPusherService pusherService, IConversationService conversationService)
        {
            _pusherService = pusherService;
            _conversationService = conversationService;
        }

        [HttpPost("auth")]
        [Authorize]
        public async Task<IActionResult> Authenticate()
        {
            var socketId = Request.Form["socket_id"];
            var channelName = Request.Form["channel_name"];

            if (!await UserHasAccessToChannel(UserId, channelName))
                return Forbid();

            var pusher = _pusherService.GetPusherClient();
            var auth = pusher.Authenticate(channelName, socketId);

            return Ok(auth);
        }

        private async Task<bool> UserHasAccessToChannel(int userId, string channelName)
        {
            if (!channelName.StartsWith("private-conversation-"))
                return false;

            var parts = channelName.Split('-');
            if (!int.TryParse(parts.Last(), out var conversationId))
                return false;

            var conversation = await _conversationService.GetConversasionByIdAsync(conversationId);
            if (conversation == null)
                return false;

            return conversation.User1Id == userId || conversation.User2Id == userId;
        }

    }
}
