using ClickFlow.BLL.DTOs.FeedbackDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FeedbacksController : BaseAPIController
	{
		private readonly IFeedbackService _feedbackService;

		public FeedbacksController(IFeedbackService feedbackService)
		{
			_feedbackService = feedbackService;
		}

		[Authorize]
		[HttpGet]
		[Route("get-all-feedbacks/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetAllFeedbacks(int pageIndex, int pageSize)
		{
			var response = await _feedbackService.GetAllFeedbacks(pageIndex, pageSize);
			return GetSuccess(response);
		}

		[Authorize]
		[HttpGet]
		[Route("get-feedbacks-by-campaign/{campaignId}/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetFeedbacksByCampaignId(int campaignId, int pageIndex, int pageSize)
		{
			var response = await _feedbackService.GetFeedbacksByCampaignId(campaignId, pageIndex, pageSize);
			return GetSuccess(response);
		}

		[Authorize]
		[HttpGet]
		[Route("get-feedbacks-by-feedbacker/{feedbackerId}/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetFeedbacksByFeedbackerId(int feedbackerId, int pageIndex, int pageSize)
		{
			var response = await _feedbackService.GetFeedbacksByFeedbackerId(feedbackerId, pageIndex, pageSize);
			return GetSuccess(response);
		}

		[Authorize]
		[HttpGet]
		[Route("get-feedback-by-id/{id}")]
		public async Task<IActionResult> GetFeedbackById(int id)
		{
			var response = await _feedbackService.GetFeedbackById(id);
			if (response == null) return GetError("Phản hồi không tồn tại.");
			return GetSuccess(response);
		}

		[Authorize]
		[HttpPost]
		[Route("create-feedback")]
		public async Task<IActionResult> CreateFeedback(FeedbackCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();
			var response = await _feedbackService.CreateFeedback(dto, UserId);
			if (!response.IsSuccess) return SaveError(response);
			return SaveSuccess(response);
		}

		[Authorize]
		[HttpPut]
		[Route("update-feedback")]
		public async Task<IActionResult> UpdateFeedback(FeedbackUpdateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();
			var response = await _feedbackService.UpdateFeedback(dto, UserId);
			if (!response.IsSuccess) return SaveError(response);
			return SaveSuccess(response);
		}

		[Authorize]
		[HttpDelete]
		[Route("delete-feedback/{id}")]
		public async Task<IActionResult> DeleteFeedback(int id)
		{
			var response = await _feedbackService.DeleteFeedback(id);
			if (!response.IsSuccess) return SaveError(response);
			return SaveSuccess(response);
		}

		[Authorize]
		[HttpGet]
		[Route("check-feedback/{campaignId}")]
		public async Task<IActionResult> CheckFeedback(int campaignId)
		{
			var hasFeedback = await _feedbackService.HasFeedback(campaignId, UserId);
			return GetSuccess(hasFeedback);
		}
	}
}
