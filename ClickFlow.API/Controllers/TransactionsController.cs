﻿using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.DTOs.VnPayDTOs;
using ClickFlow.BLL.Services.Implements;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionsController : BaseAPIController
	{
		private readonly ITransactionService _transactionService;
		private readonly IVnPayService _vnPayService;

		public TransactionsController(ITransactionService transactionService, IVnPayService vnPayService)
		{
			_transactionService = transactionService;
			_vnPayService = vnPayService;
		}

		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpPost]
		public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _transactionService.CreateTransactionAsync(dto);
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

		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpGet("own")]
		public async Task<IActionResult> GetOwnTransactions([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var userId = User.FindFirst("Id")?.Value;

				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User Id không hợp lệ hoặc chưa đăng nhập.");
				}

				var response = await _transactionService.GetAllTransactionsByUserIdAsync(int.Parse(userId), dto);
				if (response == null) return GetNotFound("Không có dữ liệu.");
				return Ok(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return StatusCode(500, "Lỗi máy chủ, vui lòng thử lại sau.");
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("status")]
		public async Task<IActionResult> UpdateStatusTransaction([FromQuery] int id, [FromBody] TransactionUpdateStatusDTO dto)
		{
			try
			{
				var response = await _transactionService.UpdateStatusTransactionAsync(id, dto);
				if (response == null) return SaveError();
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return StatusCode(500, "Lỗi máy chủ, vui lòng thử lại sau.");
			}
		}


		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpPost("payment-url")]
		public IActionResult CreatePaymentUrl([FromBody] VnPayRequestDTO dto)
		{
			try
			{
				if (dto == null || dto.Amount <= 0)
				{
					ModelState.AddModelError("Amount", "Không được nhỏ hơn 0.");
					return ModelInvalid();
				}

				var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, dto);
				return GetSuccess(paymentUrl);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[HttpGet("response-payment")]
		public async Task<IActionResult> PaymentResponse()
		{
			try
			{
				var vnpayRes = _vnPayService.PaymentExcute(Request.Query);

				var transactionRes = await _transactionService.UpdateStatusTransactionAsync(
					int.Parse(vnpayRes.OrderId), 
					new TransactionUpdateStatusDTO { Status = vnpayRes.IsSuccess });

				if (!vnpayRes.IsSuccess)
				{
					return SaveError(transactionRes);
				}
				return SaveSuccess(transactionRes);
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
		public async Task<IActionResult> GetAllTransactions([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var response = await _transactionService.GetAllTransactionsAsync(dto);
				if (response == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return StatusCode(500, "Lỗi máy chủ, vui lòng thử lại sau.");
			}
		}
	}
}
