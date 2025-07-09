using AutoMapper;
using ClickFlow.BLL.DTOs.CommentDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class CommentService : ICommentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<BaseResponse> CreateComment(CommentCreateDTO dto, int userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var commentRepo = _unitOfWork.GetRepo<Comment>();
				var postRepo = _unitOfWork.GetRepo<Post>();

				
				var post = await postRepo.GetSingleAsync(new QueryBuilder<Post>()
					.WithPredicate(p => p.Id == dto.PostId && !p.IsDeleted)
					.Build());

				if (post == null)
				{
					await _unitOfWork.RollBackAsync();
					return new BaseResponse { IsSuccess = false, Message = "Bài viết không tồn tại." };
				}

				var comment = _mapper.Map<Comment>(dto);
				comment.AuthorId = userId;
				comment.CreatedAt = DateTime.UtcNow;

				await commentRepo.CreateAsync(comment);

				
				post.FeedbackNumber += 1;
				await postRepo.UpdateAsync(post);

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Bình luận đã được tạo." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> UpdateComment(CommentUpdateDTO dto, int userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var repo = _unitOfWork.GetRepo<Comment>();
				var comment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
					.WithPredicate(x => x.Id == dto.Id && x.AuthorId == userId)
					.Build());

				if (comment == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy bình luận hoặc bạn không có quyền cập nhật." };
				}

				_mapper.Map(dto, comment);
				await repo.UpdateAsync(comment);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Bình luận đã được cập nhật." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> DeleteComment(int id)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var commentRepo = _unitOfWork.GetRepo<Comment>();
				var postRepo = _unitOfWork.GetRepo<Post>();

				var comment = await commentRepo.GetSingleAsync(new QueryBuilder<Comment>()
					.WithPredicate(x => x.Id == id)
					.Build());

				if (comment == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy bình luận." };
				}

				if (comment.IsDeleted)
				{
					return new BaseResponse { IsSuccess = false, Message = "Bình luận đã được xoá trước đó." };
				}

				comment.IsDeleted = true;
				await commentRepo.UpdateAsync(comment);

				
				var post = await postRepo.GetSingleAsync(new QueryBuilder<Post>()
					.WithPredicate(p => p.Id == comment.PostId && !p.IsDeleted)
					.Build());

				if (post != null && post.FeedbackNumber > 0)
				{
					post.FeedbackNumber -= 1;
					await postRepo.UpdateAsync(post);
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Bình luận đã được xoá." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<CommentResponseDTO> GetCommentById(int id)
		{
			var repo = _unitOfWork.GetRepo<Comment>();
			var comment = await repo.GetSingleAsync(new QueryBuilder<Comment>()
				.WithPredicate(x => x.Id == id && !x.IsDeleted)
				.WithInclude(c => c.Author)
				.WithInclude(c => c.Author.UserDetail)
				.Build());

			return comment == null ? null : _mapper.Map<CommentResponseDTO>(comment);
		}

		public async Task<PaginatedList<CommentResponseDTO>> GetCommentsByPostId(int postId, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<Comment>();
			var query = repo.Get(new QueryBuilder<Comment>()
				.WithPredicate(c => c.PostId == postId && !c.IsDeleted)
				.WithInclude(c => c.Author)
				.WithInclude(c => c.Author.UserDetail)
				.Build());

			var paged = await PaginatedList<Comment>.CreateAsync(query, pageIndex, pageSize);
			var mapped = _mapper.Map<List<CommentResponseDTO>>(paged);
			return new PaginatedList<CommentResponseDTO>(mapped, paged.TotalItems, pageIndex, pageSize);
		}
	}
}
