using ClickFlow.BLL.DTOs.WalletDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

		[Authorize(Roles = "Publisher, Advertiser")]
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
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Publisher, Advertiser")]
		[HttpGet]
		[Route("own")]
		public async Task<IActionResult> GetOwnWallet()
		{
			try
			{
				var userId = User.FindFirst("Id")?.Value;

				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User Id không hợp lệ hoặc chưa đăng nhập.");
				}

				var response = await _walletService.GetWalletByUserIdAsync(int.Parse(userId));
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

	}
}
