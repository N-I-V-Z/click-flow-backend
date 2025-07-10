using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Helpers.Config;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TrafficsController : BaseAPIController
	{
		private readonly ITrafficService _trafficService;
		private readonly ICampaignService _campaignService;
		private readonly IUserPlanService _userPlanService;


		public TrafficsController(ITrafficService trafficService, ICampaignService campaignService, IUserPlanService userPlanService)
		{
			_trafficService = trafficService;
			_campaignService = campaignService;
			_userPlanService = userPlanService;
		}

		[Authorize]
		[HttpGet("{trafficId}")]
		public async Task<IActionResult> GetTrafficById(int trafficId)
		{
			try
			{
				var response = await _trafficService.GetByIdAsync(trafficId);
				if (response == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetTraffics([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _trafficService.GetAllAsync(dto);
				var response = new PagingDTO<TrafficResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publisher")]
		public async Task<IActionResult> GetTrafficsByPublisher([FromQuery] TrafficForPublisherDTO dto)
		{
			try
			{
				var data = await _trafficService.GetAllByPublisherIdAsync(UserId, dto);
				var response = new PagingDTO<TrafficResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Advertiser")]
		[HttpGet("advertiser")]
		public async Task<IActionResult> GetTrafficsByAdvertiser([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _trafficService.GetAllByAdvertiserIdAsync(UserId, dto);
				var response = new PagingDTO<TrafficResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("campaign/{campaignId}")]
		public async Task<IActionResult> GetTrafficsByCampaign(int campaignId, [FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _trafficService.GetAllByCampaignIdAsync(campaignId, dto);
				var response = new PagingDTO<TrafficResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateTraffic([FromBody] TrafficCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				// 1) Kiểm tra campaign
				var checkCampaign = await _campaignService.ValidateCampaignForTraffic(dto.CampaignId);
				if (!checkCampaign.IsSuccess)
					return SaveError(checkCampaign.Message);

				// 2) Kiểm tra traffic hợp lệ (ví dụ publisher đã tham gia)
				var checkTraffic = await _trafficService.ValidateTrafficAsync(dto);
				if (!checkTraffic.IsSuccess)
					return SaveError(checkTraffic.Message);

				// 3) Tính IP
				var ip = Utils.GetIpAddress(HttpContext);

				// 4) Tăng click count, nếu hết quota quay về lỗi
				//    PublisherId lấy từ dto.PublisherId (giả định DTO có trường này)
				var canIncrease = await _userPlanService.IncreaseClickCountAsync(dto.PublisherId);
				if (!canIncrease)
					return SaveError("Bạn đã hết hạn mức click cho gói hiện tại.");

				// 5) Ghi nhận traffic
				var response = await _trafficService.CreateAsync(dto, ip);
				if (response == null)
					return SaveError("Lưu traffic thất bại.");

				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin, Advertiser")]
		[HttpGet("{campaignId}/count")]
		public async Task<IActionResult> GetCountTrafficByCampaignId(int campaignId)
		{
			try
			{
				var response = await _trafficService.CountAllTrafficByCampaignAsync(campaignId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publisher/{campaignId}/count")]
		public async Task<IActionResult> GetCountTrafficForPublisher(int campaignId)
		{
			try
			{
				var response = await _trafficService.CountTrafficForPublisherAsync(campaignId, UserId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publisher/active-campaign/count")]
		public async Task<IActionResult> GetCountTrafficAllActiveCampaignForPublisher()
		{
			try
			{
				var response = await _trafficService.CountTrafficOfAllActiveCampaignForPublisherAsync(UserId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publishers/statistics/browsers")]
		public async Task<IActionResult> GetBrowserStatisticsForPublisher()
		{
			try
			{
				var response = await _trafficService.GetBrowserStatisticsAsync(UserId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publishers/statistics/devices")]
		public async Task<IActionResult> GetDeviceStatisticsForPublisher()
		{
			try
			{
				var response = await _trafficService.GetDeviceStatisticsAsync(UserId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Advertiser")]
		[HttpGet("campaigns/{campaignId}/statistics/browsers")]
		public async Task<IActionResult> GetBrowserStatisticsByCampaign(int campaignId)
		{
			try
			{
				var response = await _trafficService.GetBrowserStatisticsByCampaignAsync(campaignId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Advertiser")]
		[HttpGet("campaigns/{campaignId}/statistics/devices")]
		public async Task<IActionResult> GetDeviceStatisticsByCampaign(int campaignId)
		{
			try
			{
				var response = await _trafficService.GetDeviceStatisticsByCampaignAsync(campaignId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publisher/revenue")]
		public async Task<IActionResult> GetRevenueEachCampaignForPublisher()
		{
			try
			{
				var response = await _trafficService.GetRevenuesForPublisherAsync(UserId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
