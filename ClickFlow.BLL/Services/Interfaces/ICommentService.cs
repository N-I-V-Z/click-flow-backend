using ClickFlow.BLL.DTOs.CommentDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface ICommentService
    {
        Task<BaseResponse> CreateComment(CommentCreateDTO dto, int authorId);
        Task<BaseResponse> UpdateComment(CommentUpdateDTO dto, int authorId);
        Task<BaseResponse> DeleteComment(int id);
        Task<PaginatedList<CommentResponseDTO>> GetCommentsByPostId(int postId, int pageIndex, int pageSize);
        Task<CommentResponseDTO?> GetCommentById(int id);
    }
}
