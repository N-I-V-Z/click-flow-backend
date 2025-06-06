using ClickFlow.BLL.DTOs.UserPlanDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserPlansController : BaseAPIController
	{
		private readonly IUserPlanService _userPlanService;

		public UserPlansController(IUserPlanService userPlanService)
		{
			_userPlanService = userPlanService;
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("current")]
		public async Task<IActionResult> GetCurrentPlan([FromQuery] int publisherId)
		{

			try
			{
				var response = await _userPlanService.GetCurrentPlanAsync(publisherId);
				if (response == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpPost("assign")]
		public async Task<IActionResult> AssignPlan([FromBody] UserPlanAssignDTO dto)
		{
			if (!ModelState.IsValid)
				return ModelInvalid();

			try
			{
				var result = await _userPlanService.AssignPlanToPublisherAsync(UserId, dto.PlanId);
				if (result == null) return SaveError();
				return SaveSuccess(result);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}

		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("can-add-campaign")]
		public async Task<IActionResult> CanAddCampaign()
		{
			try
			{
				bool canAdd = await _userPlanService.CanAddCampaignAsync(UserId);
				return GetSuccess(canAdd);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}


	}
}
