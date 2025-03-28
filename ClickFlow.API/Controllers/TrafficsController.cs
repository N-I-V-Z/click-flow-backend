﻿using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.Services.Implements;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TrafficsController : BaseAPIController
	{
		private readonly ITrafficService _trafficService;
		private readonly ICampaignService _campaignService;

		public TrafficsController(ITrafficService trafficService, ICampaignService campaignService)
		{
			_trafficService = trafficService;
			_campaignService = campaignService;
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

		[Authorize(Roles = "Publisher")]
		[HttpGet("publisher")]
		public async Task<IActionResult> GetTrafficsByPublisher([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _trafficService.GetAllByPublisherIdAsync(UserId, dto);
                var response = new PagingDTO<TrafficResponseDTO>(data);

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
		[HttpGet("advertiser")]
		public async Task<IActionResult> GetTrafficsByAdvertiser([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _trafficService.GetAllByAdvertiserIdAsync(UserId, dto);
                var response = new PagingDTO<TrafficResponseDTO>(data);

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

		[Authorize]
		[HttpGet("campaign/{campaignId}")]
		public async Task<IActionResult> GetTrafficsByCampaign(int campaignId, [FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var response = await _trafficService.GetAllByCampaignIdAsync(campaignId, dto);
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

		[HttpPost]
		public async Task<IActionResult> CreateTraffic([FromBody] TrafficCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var checkCampaign = await _campaignService.ValidateCampaignForTraffic(dto.CampaignId);

				if (!checkCampaign.IsSuccess)
				{
					return SaveError(checkCampaign.Message);
				}

				var checkTraffic = await _trafficService.ValidateTraffic(dto);

                if (!checkTraffic.IsSuccess)
                {
                    return SaveError(checkTraffic.Message);
                }

                var remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();

                var response = await _trafficService.CreateAsync(dto, remoteIp);
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

		[Authorize]
		[HttpGet("count/{campaignId}")]
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
