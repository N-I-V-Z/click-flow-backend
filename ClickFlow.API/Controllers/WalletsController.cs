using ClickFlow.BLL.DTOs.WalletDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

		[Authorize(Roles = "Admin, Publisher, Advertiser")]
		[HttpGet("own")]
		public async Task<IActionResult> GetOwnWallet()
		{
			try
			{
				var response = await _walletService.GetWalletByUserIdAsync(UserId);
				if (response == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin, Publisher, Advertiser")]
		[HttpPut("{walletId}")]
		public async Task<IActionResult> UpdateWallet(int walletId, [FromBody] WalletUpdateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _walletService.UpdateWalletAsync(walletId, dto);
				if (response == null) return SaveError();
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
