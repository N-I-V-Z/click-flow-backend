using Azure;
using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.PayOSDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using System.Text.Json;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionsController : BaseAPIController
	{
		private readonly ITransactionService _transactionService;
		//private readonly IVnPayService _vnPayService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPayOSService _payOSService;
		private readonly IConfiguration _configuration;

		public TransactionsController(
			ITransactionService transactionService
			/*, IVnPayService vnPayService */,
			 IHttpContextAccessor httpContextAccessor,
			 IPayOSService payOSService,
			 IConfiguration configuration)
		{
			//_vnPayService = vnPayService;
			_transactionService = transactionService;
			_httpContextAccessor = httpContextAccessor;
			_payOSService = payOSService;
			_configuration = configuration;
		}

		[Authorize(Roles = "Admin, Publisher, Advertiser")]
		[HttpGet("own")]
		public async Task<IActionResult> GetOwnTransactions([FromQuery] TransactionGetByUserIdDTO dto)
		{
			try
			{
				var data = await _transactionService.GetAllTransactionsByUserIdAsync(UserId, dto);
				var response = new PagingDTO<TransactionResponseDTO>(data);

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

		//[HttpGet("response-payment")]
		//public async Task<IActionResult> PaymentResponse()
		//{
		//	try
		//	{
		//		var vnpayRes = _vnPayService.PaymentExcute(Request.Query);

		//		if (!int.TryParse(vnpayRes.OrderId, out var transactionId))
		//		{
		//			throw new Exception("OrderId từ VnPay không hợp lệ.");
		//		}

		//		var transactionRes = await _transactionService.UpdateStatusTransactionAsync(
		//			transactionId,
		//			new TransactionUpdateStatusDTO { Status = vnpayRes.IsSuccess });

		//		if (!vnpayRes.IsSuccess)
		//		{
		//			// Thanh toán VnPay thất bại: trả thông tin lỗi
		//			return SaveError("Thanh toán không thành công: " + vnpayRes.VnPayResponseCode);
		//		}

		//		return SaveSuccess(transactionRes);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.ForegroundColor = ConsoleColor.Red;
		//		Console.WriteLine(ex.Message);
		//		Console.ResetColor();
		//		return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
		//	}
		//}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetAllTransactions([FromQuery] TransactionGetAllDTO dto)
		{
			try
			{
				var data = await _transactionService.GetAllTransactionsAsync(dto);
				var response = new PagingDTO<TransactionResponseDTO>(data);

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

		//[Authorize(Roles = "Publisher, Advertiser")]
		//[HttpPost]
		//public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDTO dto)
		//{
		//	if (!ModelState.IsValid) return ModelInvalid();

		//	try
		//	{
		//		var response = await _transactionService.CreateTransactionAsync(dto);
		//		if (response == null) return SaveError();
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

		//[Authorize(Roles = "Publisher, Advertiser")]
		//[HttpPost("payment-url")]
		//public IActionResult CreatePaymentUrl([FromBody] VnPayRequestDTO dto)
		//{
		//	if (dto == null || dto.Amount <= 0)
		//	{
		//		ModelState.AddModelError("Amount", "Không được nhỏ hơn 0.");
		//		return ModelInvalid();
		//	}

		//	try
		//	{
		//		var paymentUrl = _vnPayService.CreatePaymentUrl(UserId, HttpContext, dto);
		//		return GetSuccess(paymentUrl);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.ForegroundColor = ConsoleColor.Red;
		//		Console.WriteLine(ex.Message);
		//		Console.ResetColor();
		//		return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
		//	}
		//}

		[Authorize(Roles = "Admin")]
		[HttpPut("{transactionId}/status")]
		public async Task<IActionResult> UpdateStatusTransaction(long transactionId, [FromBody] TransactionUpdateStatusDTO dto)
		{
			try
			{
				var response = await _transactionService.UpdateStatusTransactionAsync(transactionId, dto);
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
		[HttpPost]
		public async Task<IActionResult> CreateTransactionRequest([FromBody] TransactionCreateDTO dto)
		{
			try
			{
				var response = await _transactionService.CreateTransactionAsync(UserId, dto);
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

		// PayOS
		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpPost("payos/create-payment-link")]
		public async Task<IActionResult> CreatePayOSPaymentLink([FromBody] TransactionCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var transaction = await _transactionService.CreateTransactionAsync(UserId, dto);
				if (transaction == null) return SaveError("Tạo Transaction thất bại.");

				long orderCode = transaction.Id;
				string description = $"Nạp tiền cho User: #{UserId}";
				ItemData item = new ItemData(description, 1, transaction.Amount);
				List<ItemData> items = new List<ItemData> { item };

				// Get the current request's base URL
				var feURL = _configuration["FronendURL"];

				PaymentData paymentData = new PaymentData(
					orderCode: orderCode,
					amount: transaction.Amount,
					description: description,
					items: items,
					cancelUrl: $"{feURL}/payment-cancel",
					returnUrl: $"{feURL}/payment-success"
				);

				CreatePaymentResult createPayment = await _payOSService.CreatePaymentLinkAsync(paymentData);
				return GetSuccess(createPayment.checkoutUrl);
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
		[HttpGet("payos/get-payment-link-infomation/{orderId}")]
		public async Task<IActionResult> GetPaymentLinkInformation(long orderId)
		{
			try
			{
				var response = await _payOSService.GetPaymentLinkInformationAsync(orderId);
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

		[HttpPost("payos/webhook")]
		public async Task<IActionResult> HandleWebhook([FromBody] WebhookType payload)
		{
			try
			{
				var webhookData = _payOSService.VerifyPaymentWebhookData(payload);
				if (webhookData == null)
				{
					return Ok(new PayOSWebhookResponse(-1, "fail", null));
				}

				var checkUpdateTransaction = await _transactionService.UpdateStatusTransactionAsync(webhookData.orderCode, new TransactionUpdateStatusDTO { Status = payload.success });

				if (checkUpdateTransaction == null)
				{
					return Ok(new PayOSWebhookResponse(-1, "fail", null));
				}

				return Ok(new PayOSWebhookResponse(0, "Ok", null));
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"[Webhook Error] {ex.Message}");
				Console.ResetColor();
				return Ok(new PayOSWebhookResponse(-1, "fail", null));
			}
		}

		[HttpPut("payos/cancel-payment-link/{orderId}")]
		public async Task<IActionResult> CancelPaymentLink(long orderId)
		{
			try
			{
				var response = await _payOSService.CancelPaymentLinkAsync(orderId);

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

	}
}
