using ClickFlow.BLL.DTOs.VideoDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VideosController : BaseAPIController
	{
		private readonly IVideoService _videoService;

		public VideosController(IVideoService videoService)
		{
			_videoService = videoService;
		}

		[Authorize(Roles = "Admin, Publisher")]
		[HttpGet("course/{courseId}")]
		public async Task<IActionResult> GetAllByCourseId(int courseId)
		{
			try
			{
				var response = await _videoService.GetAllVideosByCourseIdAsync(courseId);
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin, Publisher")]
		[HttpGet("{videoId}")]
		public async Task<IActionResult> GetById(int videoId)
		{
			try
			{
				var response = await _videoService.GetVideoByIdAsync(videoId);

				if (response == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> CreateVideo([FromBody] VideoCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _videoService.CreateVideoAsync(dto);
				if (response == null) return SaveError();
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{videoId}")]
		public async Task<IActionResult> UpdateVideo(int videoId, [FromBody] VideoUpdateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _videoService.UpdateVideoAsync(videoId, dto);
				if (response == null) return SaveError();
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{videoId}")]
		public async Task<IActionResult> DeleteVideo(int videoId)
		{
			try
			{
				var response = await _videoService.DeleteVideoAsync(videoId);
				return response ? SaveSuccess(response) : SaveError(response);
			}
			catch(KeyNotFoundException knfEx)
			{
				return Error(knfEx.Message);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
