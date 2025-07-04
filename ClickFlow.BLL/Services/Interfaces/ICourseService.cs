using ClickFlow.BLL.DTOs.CourseDTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ICourseService
	{
		Task<CourseResponseDTO> CreateCourseAsync(int userId, CourseCreateDTO dto);
		Task<CourseResponseDTO> UpdateCourseAsync(int id, CourseUpdateDTO dto);
		Task<PaginatedList<CourseResponseDTO>> GetAllCoursesAsync(PagingRequestDTO dto);
		Task<PaginatedList<CourseResponseDTO>> GetJoinedCoursesAsync(int userId, PagingRequestDTO dto);
		Task<CourseResponseDTO> GetCourseByIdAsync(int id);
		Task<PaginatedList<CourseResponseDTO>> GetAllCourseForPublisherAsync(int publisherId, PagingRequestDTO dto);
		Task<BaseResponse> JoinTheCourseAsync(int courseId, int publisherId);
		Task<BaseResponse> RateTheCourseAsync(int courseId, int publisherId, CourseRateDTO dto);
		Task<bool> CheckPublisherInCourseAsync(int publisherId, int courseId);
		Task<bool> DeleteCourseAsync(int courseId);
		Task<bool> CheckRateCourseAsync(int courseId, int publisherId);
	}
}
