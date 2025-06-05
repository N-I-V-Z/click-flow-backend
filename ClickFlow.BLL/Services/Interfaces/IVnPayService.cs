using ClickFlow.BLL.DTOs.VnPayDTOs;
using Microsoft.AspNetCore.Http;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IVnPayService
	{
		Task<string> CreatePaymentUrl(int userId, HttpContext context, VnPayRequestDTO vnPayRequest);
		VnPayResponseDTO PaymentExcute(IQueryCollection collection);
	}
}
