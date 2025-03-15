using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.ReportDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : BaseAPIController
	{
		private readonly IReportService _reportService;

		public ReportsController(IReportService reportService)
		{
			_reportService = reportService;
		}

		[Authorize(Roles = "Admin, Advertiser")]
		[HttpGet("{reportId}")]
		public async Task<IActionResult> GetReportById(int reportId)
		{
			try
			{
				var response = await _reportService.GetByIdAsync(reportId);
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

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetAllReports([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var response = await _reportService.GetAllAsync(dto);
				if (!response.Any()) return GetNotFound("Không có dữ liệu.");
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

        [Authorize(Roles = "Admin")]
        [HttpGet("status")]
        public async Task<IActionResult> GetReportsByStatus([FromQuery] ReportsGetByStatusDTO dto)
        {
            try
            {
                var response = await _reportService.GetByStatusAsync(dto);
                if (!response.Any()) return GetNotFound("Không có dữ liệu.");
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
		[HttpPost]
		public async Task<IActionResult> CreateReport([FromBody] ReportCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
                var advertiserId = User.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(advertiserId))
                {
                    return Unauthorized("User Id không hợp lệ hoặc chưa đăng nhập.");
                }

                var response = await _reportService.CreateReportAsync(int.Parse(advertiserId), dto);
				if (response == null) return SaveError();
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Admin, Advertiser")]
		[HttpPut("status/{reportId}")]
		public async Task<IActionResult> UpdateStatusReport(int reportId, [FromBody] ReportStatus status)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _reportService.UpdateStatusReportAsync(reportId, status);
				if (response == null) return SaveError();
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("response/{reportId}")]
		public async Task<IActionResult> UpdateResponseReport(int reportId, [FromBody] string response)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var result = await _reportService.UpdateResponseReportAsync(reportId, response);
				if (result == null) return SaveError();
				return SaveSuccess(response);
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
