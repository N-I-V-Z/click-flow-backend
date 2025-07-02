using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.CourseDTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CoursesController : BaseAPIController
	{
		private readonly ICourseService _courseService;

		public CoursesController(ICourseService courseService)
		{
			_courseService = courseService;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("admin")]
		public async Task<IActionResult> GetAllCourses([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _courseService.GetAllCoursesAsync(dto);
				var response = new PagingDTO<CourseResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("{courseId}/check")]
		public async Task<IActionResult> CheckJoinCourse(int courseId)
		{
			try
			{
				var response = await _courseService.CheckPublisherInCourseAsync(UserId, courseId);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("joined")]
		public async Task<IActionResult> GetJoinedCourses([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _courseService.GetJoinedCoursesAsync(UserId, dto);
				var response = new PagingDTO<CourseResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Publisher")]
		[HttpGet("publisher")]
		public async Task<IActionResult> GetAllCoursesForPublisher([FromQuery] PagingRequestDTO dto)
		{
			try
			{
				var data = await _courseService.GetAllCourseForPublisherAsync(UserId, dto);
				var response = new PagingDTO<CourseResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Admin, Publisher")]
		[HttpGet("{courseId}")]
		public async Task<IActionResult> GetById(int courseId)
		{
			try
			{
				var response = await _courseService.GetCourseByIdAsync(courseId);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _courseService.CreateCourseAsync(UserId, dto);
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

		[Authorize(Roles = "Publisher")]
		[HttpPost("{courseId}/join")]
		public async Task<IActionResult> JoinCourse(int courseId)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _courseService.JoinTheCourseAsync(courseId, UserId);
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

		[Authorize(Roles = "Publisher")]
		[HttpPost("{courseId}/rate")]
		public async Task<IActionResult> RateCourse(int courseId, [FromBody] CourseRateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _courseService.RateTheCourseAsync(courseId, UserId, dto);
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

		[Authorize(Roles = "Admin")]
		[HttpPut("{courseId}")]
		public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] CourseUpdateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();

			try
			{
				var response = await _courseService.UpdateCourseAsync(courseId, dto);
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
	}
}
