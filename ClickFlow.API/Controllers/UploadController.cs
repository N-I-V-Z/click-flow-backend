using ClickFlow.BLL.DTOs.UploadImageDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UploadController : ControllerBase
	{
		private readonly ICloudinaryService _cloudinaryService;

		public UploadController(ICloudinaryService cloudinaryService)
		{
			_cloudinaryService = cloudinaryService;
		}

		[HttpPost("upload-image")]
		public async Task<IActionResult> UploadImage([FromForm] UploadImageDTO dto)
		{
			try
			{
				if (dto.File == null || dto.File.Length == 0)
				{
					return BadRequest("File không hợp lệ.");
				}

				var uploadResult = await _cloudinaryService.UploadImageAsync(dto.File);

				return Ok(new
				{
					PublicId = uploadResult.PublicId,
					Url = uploadResult.Url.ToString()
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Đã xảy ra lỗi khi upload ảnh: {ex.Message}");
			}
		}

		[HttpDelete("delete-image/{publicId}")]
		public async Task<IActionResult> DeleteImage(string publicId)
		{
			try
			{
				var deleteResult = await _cloudinaryService.DeleteImageAsync(publicId);
				return Ok(new { Message = "Xóa ảnh thành công.", Result = deleteResult });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Đã xảy ra lỗi khi xóa ảnh: {ex.Message}");
			}
		}
	}
}
