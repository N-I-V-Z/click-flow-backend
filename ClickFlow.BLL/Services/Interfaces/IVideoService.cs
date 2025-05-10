using ClickFlow.BLL.DTOs.VideoDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IVideoService
	{
		Task<VideoResponseDTO> CreateVideoAsync(VideoCreateDTO dto);
		Task<VideoResponseDTO> UpdateVideoAsync(int id, VideoUpdateDTO dto);
		Task<List<VideoResponseDTO>> GetAllVideosByCourseIdAsync(int courseId);
		Task<VideoResponseDTO> GetVideoByIdAsync(int id);
	}
}
