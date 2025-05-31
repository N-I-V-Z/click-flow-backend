using ClickFlow.BLL.DTOs.MessageDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MessagesController : BaseAPIController
	{
		private readonly IChatService _chatService;
		public MessagesController(IChatService chatService) 
		{
			_chatService = chatService;
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> SendMessage([FromForm] MessageSendDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _chatService.SendMessageAsync(UserId, dto);
				if (response == null) return SaveError();
				return SaveSuccess(response);

			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng kí. Hãy thử lại sau một ít phút nữa.");
			}
		}

		[Authorize]
		[HttpGet("{conversationId}")]
		public async Task<IActionResult> GetMessageByConversationId(int conversationId)
		{
			try
			{
				var response = await _chatService.GetMessagesAsync(conversationId);

				return GetSuccess(response);

			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng kí. Hãy thử lại sau một ít phút nữa.");
			}
		}

		[Authorize]
		[HttpPost("mark-as-read")] 
		public async Task<IActionResult> MaskAsRead([FromBody] MessageMaskAsReadDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				await _chatService.MarkMessagesAsReadAsync(dto.ConversationId, UserId);
				return SaveSuccess(true);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình đăng kí. Hãy thử lại sau một ít phút nữa.");
			}
		}

	}
}
