using AutoMapper;
using ClickFlow.BLL.DTOs.LikeDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class LikeService : ILikeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public LikeService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<BaseResponse> LikePost(int postId, int userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var likeRepo = _unitOfWork.GetRepo<Like>();
				var postRepo = _unitOfWork.GetRepo<Post>();

			
				var existingLikeAny = await likeRepo.GetSingleAsync(new QueryBuilder<Like>()
					.WithPredicate(l => l.PostId == postId && l.UserId == userId)
					.Build());

				if (existingLikeAny != null && !existingLikeAny.IsDeleted)
				{
					return new BaseResponse { IsSuccess = false, Message = "Bạn đã thích bài viết này rồi." };
				}

		
				if (existingLikeAny != null && existingLikeAny.IsDeleted)
				{
					existingLikeAny.IsDeleted = false;
					existingLikeAny.CreatedAt = DateTime.UtcNow;
					await likeRepo.UpdateAsync(existingLikeAny);
				}
				else
				{
					// Tạo like mới
					var like = new Like
					{
						PostId = postId,
						UserId = userId,
						CreatedAt = DateTime.UtcNow,
						IsDeleted = false
					};
					await likeRepo.CreateAsync(like);
				}

				// Cập nhật số lượng like của post
				var post = await postRepo.GetSingleAsync(new QueryBuilder<Post>()
					.WithPredicate(p => p.Id == postId && !p.IsDeleted)
					.Build());
				if (post != null)
				{
					post.LikeCount++;
					await postRepo.UpdateAsync(post);
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Đã thích bài viết thành công." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> UnlikePost(int postId, int userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var likeRepo = _unitOfWork.GetRepo<Like>();
				var postRepo = _unitOfWork.GetRepo<Post>();

				var existingLike = await likeRepo.GetSingleAsync(new QueryBuilder<Like>()
					.WithPredicate(l => l.PostId == postId && l.UserId == userId && !l.IsDeleted)
					.Build());

				if (existingLike == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Bạn chưa thích bài viết này." };
				}

				existingLike.IsDeleted = true;
				await likeRepo.UpdateAsync(existingLike);

				// Cập nhật số lượng like của post
				var post = await postRepo.GetSingleAsync(new QueryBuilder<Post>()
					.WithPredicate(p => p.Id == postId && !p.IsDeleted)
					.Build());

				if (post != null)
				{
					post.LikeCount = Math.Max(0, post.LikeCount - 1);
					await postRepo.UpdateAsync(post);
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = true, Message = "Đã bỏ thích bài viết thành công." };
			}
			catch
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<bool> IsPostLikedByUser(int postId, int userId)
		{
			var likeRepo = _unitOfWork.GetRepo<Like>();
			var like = await likeRepo.GetSingleAsync(new QueryBuilder<Like>()
				.WithPredicate(l => l.PostId == postId && l.UserId == userId && !l.IsDeleted)
				.Build());

			return like != null;
		}

		public async Task<PaginatedList<LikeResponseDTO>> GetLikesByPostId(int postId, int pageIndex, int pageSize)
		{
			var likeRepo = _unitOfWork.GetRepo<Like>();
			var likes = likeRepo.Get(new QueryBuilder<Like>()
				.WithPredicate(l => l.PostId == postId && !l.IsDeleted)
				.WithInclude(l => l.User.UserDetail)
				.Build());

			var pagedLikes = await PaginatedList<Like>.CreateAsync(likes, pageIndex, pageSize);
			var result = _mapper.Map<List<LikeResponseDTO>>(pagedLikes);
			return new PaginatedList<LikeResponseDTO>(result, pagedLikes.TotalItems, pageIndex, pageSize);
		}

		public async Task<int> GetLikeCountByPostId(int postId)
		{
			var likeRepo = _unitOfWork.GetRepo<Like>();
			var likes = likeRepo.Get(new QueryBuilder<Like>()
				.WithPredicate(l => l.PostId == postId && !l.IsDeleted)
				.Build());

			return likes.Count();
		}
	}
} 