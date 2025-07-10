using ClickFlow.BLL.DTOs.ConversationDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConversationsController : BaseAPIController
	{
		private readonly IChatService _chatService;

		public ConversationsController(IChatService chatService)
		{
			_chatService = chatService;
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateOrGetConversation([FromBody] ConversationCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _chatService.GetOrCreateConversationAsync(UserId, dto.TargetUserId);
				if (response == null) return GetError();
				return GetSuccess(response);

			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("own")]
		public async Task<IActionResult> GetConversationsByUser()
		{
			try
			{
				var response = await _chatService.GetUserConversationsAsync(UserId);

				return GetSuccess(response);

			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
