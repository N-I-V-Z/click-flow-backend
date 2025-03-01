using ClickFlow.BLL.DTOs.VnPayDTOs;
using Microsoft.AspNetCore.Http;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPayRequestDTO vnPayRequest);
		VnPayResponseDTO PaymentExcute(IQueryCollection collection);
	}
}
