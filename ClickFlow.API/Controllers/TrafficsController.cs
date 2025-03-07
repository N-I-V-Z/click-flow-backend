using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Services.Implements;
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

		public TrafficsController(ITrafficService trafficService)
		{
			_trafficService = trafficService;
		}

		[Authorize]
		[HttpGet("id")]
		public async Task<IActionResult> GetTrafficById([FromQuery] int id)
		{
			try
			{
				var response = await _trafficService.GetByIdAsync(id);
				if (response == null) return GetError("Không có dữ liệu.");
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
				var response = await _trafficService.GetAllAsync(dto);
				if (!response.Any()) return GetError("Không có dữ liệu.");
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
		public async Task<IActionResult> GetTrafficsByPublisher([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var userId = User.FindFirst("Id")?.Value;

				var response = await _trafficService.GetAllByPublisherIdAsync(int.Parse(userId), dto);
				if (!response.Any()) return GetError("Không có dữ liệu.");
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
				var userId = User.FindFirst("Id")?.Value;

				var response = await _trafficService.GetAllByPublisherIdAsync(int.Parse(userId), dto);
				if (!response.Any()) return GetError("Không có dữ liệu.");
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

		//[HttpPost]
		//public async Task<IActionResult> CreateTraffic([FromBody] TrafficCreateDTO dto)
		//{
		//	if (!ModelState.IsValid) return ModelInvalid();

		//	try
		//	{

		//		var response = await _trafficService.CreateAsync(dto);
		//		if (response == null) return SaveError(response);
		//		return SaveSuccess(response);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.ForegroundColor = ConsoleColor.Red;
		//		Console.WriteLine(ex.Message);
		//		Console.ResetColor();
		//		return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
		//	}
		//}
	}
}
