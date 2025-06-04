using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.ReportDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		public async Task<IActionResult> GetAllReports([FromQuery] ReportGetAllDTO dto)
        {
            try
            {
				var data = await _reportService.GetAllAsync(dto);
                var response = new PagingDTO<ReportResponseDTO>(data);

                if (!data.Any()) return GetNotFound("Không có dữ liệu.");
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
                var response = await _reportService.CreateReportAsync(UserId, dto);
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
		[HttpPut("{reportId}/status")]
		public async Task<IActionResult> UpdateStatusReport(int reportId, [FromBody] ReportStatusDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _reportService.UpdateStatusReportAsync(reportId, dto.Status);
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
		[HttpPut("{reportId}/response")]
		public async Task<IActionResult> UpdateResponseReport(int reportId, [FromBody] ReportUpdateResponseDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var result = await _reportService.UpdateResponseReportAsync(reportId, dto.Response);
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
	}
}
