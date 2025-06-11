using ClickFlow.BLL.Services.Interfaces;
using Net.payOS;
using Net.payOS.Types;

namespace ClickFlow.BLL.Services.Implements
{
	public class PayOSService : IPayOSService
	{
		private readonly PayOS _payOS;
		public PayOSService(PayOS payOS)
		{
			_payOS = payOS;
		}

		public async Task<PaymentLinkInformation> CancelPaymentLinkAsync(long orderId)
		{
			try
			{
				PaymentLinkInformation response = await _payOS.cancelPaymentLink(orderId);
				return response;
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task ConfirmWebhook(string webhookUrl)
		{
			try
			{
				await _payOS.confirmWebhook(webhookUrl);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public Task ConfirmWebhookAsync(string webhookUrl)
		{
			throw new NotImplementedException();
		}

		public async Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentData data)
		{
			try
			{
				CreatePaymentResult response = await _payOS.createPaymentLink(data);
				if (response == null) throw new Exception("Lỗi khi tạo link thanh toán.");

				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<PaymentLinkInformation> GetPaymentLinkInformation(long orderId)
		{
			try
			{
				PaymentLinkInformation response = await _payOS.getPaymentLinkInformation(orderId);
				if (response == null) throw new Exception($"Lỗi khi lấy thông tin link thanh toán Order: #{orderId}.");

				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderId)
		{
			try
			{
				PaymentLinkInformation response = await _payOS.cancelPaymentLink(orderId);
				if (response == null) throw new Exception($"Lỗi khi hủy thông tin link thanh toán Order: #{orderId}.");

				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public WebhookData VerifyPaymentWebhookData(WebhookType data)
		{
			try
			{
				WebhookData response = _payOS.verifyPaymentWebhookData(data);

				if (data.desc == "Ma giao dich thu nghiem" || data.desc == "VQRIO123")
				{
					return response;
				}

				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
