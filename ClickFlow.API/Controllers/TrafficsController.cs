using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Helpers.Config;
using ClickFlow.BLL.Services.Interfaces;
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
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
				var checkTraffic = await _trafficService.ValidateTraffic(dto);
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi. Vui lòng thử lại sau.");
			}
		}

		[Authorize]
		[HttpGet("{campaignId}/count")]
		public async Task<IActionResult> GetCountTrafficByCampaignId(int campaignId)
		{
			try
			{
				var response = await _trafficService.CountAllTrafficByCampaign(campaignId);
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
	}
}
