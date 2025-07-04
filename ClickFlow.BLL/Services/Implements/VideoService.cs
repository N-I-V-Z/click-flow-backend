using AutoMapper;
using ClickFlow.BLL.DTOs.VideoDTOs;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class VideoService : IVideoService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public VideoService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		protected virtual QueryBuilder<Video> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<Video>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<Video>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<VideoResponseDTO> CreateVideoAsync(VideoCreateDTO dto)
		{
			var videoRepo = _unitOfWork.GetRepo<Video>();
			var newVideo = _mapper.Map<Video>(dto);

			await videoRepo.CreateAsync(newVideo);

			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<VideoResponseDTO>(newVideo);
		}

		public async Task<List<VideoResponseDTO>> GetAllVideosByCourseIdAsync(int courseId)
		{
			var videoRepo = _unitOfWork.GetRepo<Video>();
			var queryBuilder = CreateQueryBuilder().WithPredicate(x => x.CourseId == courseId);

			var loadedRecords = await videoRepo.GetAllAsync(queryBuilder.Build());

			return _mapper.Map<List<VideoResponseDTO>>(loadedRecords);
		}

		public async Task<VideoResponseDTO> GetVideoByIdAsync(int id)
		{
			var videoRepo = _unitOfWork.GetRepo<Video>();
			var queryBuilder = CreateQueryBuilder().WithPredicate(x => x.Id == id);

			var loadedRecords = await videoRepo.GetSingleAsync(queryBuilder.Build());

			return _mapper.Map<VideoResponseDTO>(loadedRecords);
		}

		public async Task<VideoResponseDTO> UpdateVideoAsync(int id, VideoUpdateDTO dto)
		{
			var videoRepo = _unitOfWork.GetRepo<Video>();

			var video = await videoRepo.GetSingleAsync(CreateQueryBuilder()
				.WithPredicate(x => x.Id == id)
				.WithTracking(false)
				.Build());

			if (videoRepo == null)
			{
				return null;
			}

			_mapper.Map(dto, video);
			await videoRepo.UpdateAsync(video);

			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<VideoResponseDTO>(video);
		}

		public async Task<bool> DeleteVideoAsync(int id)
		{
			var videoRepo = _unitOfWork.GetRepo<Video>();

			var query = CreateQueryBuilder().WithPredicate(x => x.Id == id);

			var video = await videoRepo.GetSingleAsync(query.Build());

			if (video == null) throw new KeyNotFoundException("Không tìm thấy video.");

			await videoRepo.DeleteAsync(video);

			var saver = await _unitOfWork.SaveAsync();

			return saver == true ? true : false;
		}
	}
}
