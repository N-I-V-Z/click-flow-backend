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
		public async Task<IActionResult> GetCurrentPlan()
		{
			try
			{
				var response = await _userPlanService.GetCurrentPlanAsync(UserId);
				if (response == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
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
			catch (KeyNotFoundException knfEx)
			{
				return Error(knfEx.Message);
			}
			catch (InvalidOperationException ioEx)
			{
				return Error(ioEx.Message);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
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
				return Error(ex.Message);
			}
		}


	}
}
