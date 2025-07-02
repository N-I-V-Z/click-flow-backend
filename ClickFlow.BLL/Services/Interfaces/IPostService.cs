using ClickFlow.BLL.DTOs.PostDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IPostService
	{
		Task<BaseResponse> CreatePost(PostCreateDTO dto, int authorId);
		Task<BaseResponse> UpdatePost(PostUpdateDTO dto, int authorId);
		Task<BaseResponse> DeletePost(int id);
		Task<PaginatedList<PostResponseDTO>> GetAllPosts(int pageIndex, int pageSize, int? currentUserId = null, bool isAscending = false);
		Task<PaginatedList<PostResponseDTO>> GetPostsByAuthorId(int authorId, int pageIndex, int pageSize, int? currentUserId = null, bool isAscending = false);
		Task<PostResponseDTO> GetPostById(int id, int? currentUserId = null);
		Task<PaginatedList<PostResponseDTO>> SearchPosts(PostSearchDTO searchDto, int pageIndex, int pageSize, int? currentUserId = null);
	}
}
