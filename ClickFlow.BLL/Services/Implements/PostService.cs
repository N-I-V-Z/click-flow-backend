using AutoMapper;
using ClickFlow.BLL.DTOs.PostDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class PostService : IPostService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PostService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<BaseResponse> CreatePost(PostCreateDTO dto, int authorId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Post>();

				var post = _mapper.Map<Post>(dto);
				post.AuthorId = authorId;
				post.CreatedAt = DateTime.Now;
				post.IsDeleted = false;
				post.View = 0;
				post.FeedbackNumber = 0;

				await repo.CreateAsync(post);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Bài viết đã đuocự tạo thành công" };


			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> DeletePost(int id)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Post>();

				var post = await repo.GetSingleAsync(new QueryBuilder<Post>()
					.WithPredicate(p => p.Id == id).Build());

				if (post == null) return new BaseResponse { IsSuccess = false, Message = "Bài viết không tồn tại." };

				post.IsDeleted = true;
				await repo.UpdateAsync(post);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Bài viết đã được xóa mềm." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<PaginatedList<PostResponseDTO>> GetAllPosts(int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<Post>();
			var posts = repo.Get(new QueryBuilder<Post>()
				.WithPredicate(p => !p.IsDeleted)
				.WithInclude(p => p.Author.UserDetail)
				.Build());

			var pagedPosts = await PaginatedList<Post>.CreateAsync(posts, pageIndex, pageSize);
			var result = _mapper.Map<List<PostResponseDTO>>(pagedPosts);
			return new PaginatedList<PostResponseDTO>(result, pagedPosts.TotalPages, pageSize, pageIndex);
		}

		public async Task<PostResponseDTO> GetPostById(int id)
		{
			var repo = _unitOfWork.GetRepo<Post>();
			var post = await repo.GetSingleAsync(new QueryBuilder<Post>()
				.WithPredicate(p => p.Id == id && !p.IsDeleted)
				.WithInclude(p => p.Author.UserDetail)
				.Build());
			return post == null ? null : _mapper.Map<PostResponseDTO>(post);
		}

		public async Task<PaginatedList<PostResponseDTO>> GetPostsByAuthorId(int authorId, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<Post>();
			var posts = repo.Get(new QueryBuilder<Post>()
				.WithPredicate(p => p.AuthorId == authorId && !p.IsDeleted)
				.WithInclude(p => p.Author.UserDetail)
				.Build());

			var pagedPosts = await PaginatedList<Post>.CreateAsync(posts, pageIndex, pageSize);
			var result = _mapper.Map<List<PostResponseDTO>>(pagedPosts);
			return new PaginatedList<PostResponseDTO>(result, pagedPosts.TotalItems, pageIndex, pageSize);
		}

		public async Task<BaseResponse> UpdatePost(PostUpdateDTO dto, int authorId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Post>();

				var post = await repo.GetSingleAsync(new QueryBuilder<Post>()
					.WithPredicate(p => p.Id == dto.Id && p.AuthorId == authorId)
					.WithTracking(false)
					.Build());

				if (post == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Bài viết không tồn tại hoặc bạn không có quyền chỉnh sửa." };
				}

				_mapper.Map(dto, post);
				await repo.UpdateAsync(post);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Bài viết đã được cập nhật thành công." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}

		}

		public async Task<PaginatedList<PostResponseDTO>> SearchPosts(PostSearchDTO searchDto, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<Post>();
			var queryBuilder = new QueryBuilder<Post>()
				.WithPredicate(p => !p.IsDeleted);

			if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
			{
				queryBuilder = queryBuilder.WithPredicate(p => !p.IsDeleted && (p.Title.Contains(searchDto.Keyword) || p.Content.Contains(searchDto.Keyword)));
			}
			if (searchDto.Topic.HasValue)
			{
				queryBuilder = queryBuilder.WithPredicate(p => !p.IsDeleted && p.Topic == searchDto.Topic);
			}
			if (searchDto.AuthorId.HasValue)
			{
				queryBuilder = queryBuilder.WithPredicate(p => !p.IsDeleted && p.AuthorId == searchDto.AuthorId);
			}
			queryBuilder = queryBuilder.WithInclude(p => p.Author.UserDetail);

			var posts = repo.Get(queryBuilder.Build());
			var pagedPosts = await PaginatedList<Post>.CreateAsync(posts, pageIndex, pageSize);
			var result = _mapper.Map<List<PostResponseDTO>>(pagedPosts);
			return new PaginatedList<PostResponseDTO>(result, pagedPosts.TotalItems, pageIndex, pageSize);
		}
	}
}
