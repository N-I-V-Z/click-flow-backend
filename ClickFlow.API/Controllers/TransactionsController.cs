using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.PayOSDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

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

		public TransactionsController(
			ITransactionService transactionService
			/*, IVnPayService vnPayService */,
			 IHttpContextAccessor httpContextAccessor,
			 IPayOSService payOSService)
		{
			//_vnPayService = vnPayService;
			_transactionService = transactionService;
			_httpContextAccessor = httpContextAccessor;
			_payOSService = payOSService;
		}

		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpGet("own")]
		public async Task<IActionResult> GetOwnTransactions([FromQuery] PagingRequestDTO dto)
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
		public async Task<IActionResult> GetAllTransactions([FromQuery] PagingRequestDTO dto)
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

		// PayOS
		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpPost("payos/create-payment-link")]
		public async Task<IActionResult> CreatePayOSPaymentLink(TransactionCreateDTO dto)
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
				var request = _httpContextAccessor.HttpContext.Request;
				var baseUrl = $"{request.Scheme}://{request.Host}";

				PaymentData paymentData = new PaymentData(
					orderCode: orderCode,
					amount: transaction.Amount,
					description: description,
					items: items,
					cancelUrl: $"{baseUrl}/cancel",
					returnUrl: $"{baseUrl}/success"
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
		public async Task<IActionResult> GetPaymentLinkInformation(int orderId)
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
		public async Task<IActionResult> HandleWebhook(ConfirmWebhookDTO dto)
		{
			try
			{
				await _payOSService.ConfirmWebhookAsync(dto.webhook_url);

				return SaveSuccess(true);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return StatusCode(500, "Lỗi máy chủ, vui lòng thử lại sau.");
			}
		}


		[HttpPut("payos/cancel-payment-link/{orderId}")]
		public async Task<IActionResult> CancelPaymentLink(int orderId)
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
