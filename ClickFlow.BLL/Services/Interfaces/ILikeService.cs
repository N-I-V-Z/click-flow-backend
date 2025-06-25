using ClickFlow.BLL.DTOs.LikeDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ILikeService
	{
		Task<BaseResponse> LikePost(int postId, int userId);
		Task<BaseResponse> UnlikePost(int postId, int userId);
		Task<bool> IsPostLikedByUser(int postId, int userId);
		Task<PaginatedList<LikeResponseDTO>> GetLikesByPostId(int postId, int pageIndex, int pageSize);
		Task<int> GetLikeCountByPostId(int postId);
	}
} 