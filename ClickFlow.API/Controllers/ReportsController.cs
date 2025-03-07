﻿using ClickFlow.BLL.DTOs.PagingDTOs;
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
		[HttpGet("id")]
		public async Task<IActionResult> GetReportById([FromQuery] int id)
		{
			try
			{
				var response = await _reportService.GetByIdAsync(id);
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

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetAllReports([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var response = await _reportService.GetAllAsync(dto);
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
		[HttpPost]
		public async Task<IActionResult> CreateReport([FromBody] ReportCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _reportService.CreateReportAsync(dto);
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
		[HttpDelete]
		public async Task<IActionResult> DeleteReport([FromQuery] int id)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _reportService.DeleteAsync(id);
				if (!response) return SaveError();
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
		[HttpPut("status")]
		public async Task<IActionResult> UpdateStatusReport([FromQuery] int id, [FromBody] ReportStatus status)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _reportService.UpdateStatusReportAsync(id, status);
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
		[HttpPut("response")]
		public async Task<IActionResult> UpdateResponseReport([FromQuery] int id, [FromBody] string response)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var result = await _reportService.UpdateResponseReportAsync(id, response);
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
