using ClickFlow.BLL.DTOs.WalletDTOs;
using ClickFlow.BLL.Services.Interfaces;
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

		//[Authorize]
		[HttpPost]
		//[Route("create")]
		public async Task<IActionResult> CreateWallet([FromBody] WalletCreateDTO dto)
		{
			try
			{
				var response = await _walletService.CreateWalletAsync(dto);
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
		//[Route("update")]
		public async Task<IActionResult> Update([FromQuery] int id, [FromBody] WalletUpdateDTO dto)
		{
			try
			{
				var response = await _walletService.UpdateWalletAsync(id, dto);
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

		[Authorize]
		[HttpGet]
		[Route("get-owner-wallet")]
		public async Task<IActionResult> GetWalletByToken()
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
				{
					return Unauthorized("User Id không hợp lệ hoặc chưa đăng nhập.");
				}

				var response = await _walletService.GetWalletByUserIdAsync(int.Parse(userId));
				if (response == null) return NotFound("Không tìm thấy ví cho người dùng này.");
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
