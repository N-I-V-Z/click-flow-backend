using ClickFlow.BLL.DTOs.WalletDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WalletsController : BaseAPIController
	{
		private readonly IWalletService _walletService;

		public WalletsController(IWalletService walletService)
		{
			_walletService = walletService;
		}

		//[Authorize]
		[HttpPost]
		[Route("create")]
		public async Task<IActionResult> CreateWallet([FromBody] WalletCreateDTO dto)
		{
			try
			{
				var response = await _walletService.CreateWallet(dto);
				if (response == null) return SaveError(response);
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

		//[Authorize]
		[HttpPut]
		[Route("")]
		public async Task<IActionResult> Update([FromQuery] int id, [FromBody] WalletUpdateDTO dto)
		{
			try
			{
				var response = await _walletService.UpdateWallet(id, dto);
				if (response == null) return SaveError(response);
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
