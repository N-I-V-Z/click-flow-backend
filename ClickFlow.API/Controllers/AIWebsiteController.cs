using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AIWebsiteController : BaseAPIController
	{
		private readonly IAIWebsiteService _aiWebsiteService;
		public AIWebsiteController(IAIWebsiteService aiWebsiteService)
		{
			_aiWebsiteService = aiWebsiteService;
		}
		[HttpGet("response")]
		public async Task<IActionResult> GetAIWebsiteResponse(string question)
		{
			try
			{
				var result = await _aiWebsiteService.GetWebsiteAIResponseAsync(question);
				return GetSuccess(result);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
