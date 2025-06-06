using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.UserDetailDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IUserDetailService
    {
        Task<BaseResponse> CreateUpdateUserDetail(UserDetailRequestDTO dto, int userId);
        Task<BaseResponse> DeleteUserDetail(int userId);
        Task<PaginatedList<UserDetailResponseDTO>> GetAllUserDetails(int pageIndex, int pageSize);
        Task<PaginatedList<UserDetailResponseDTO>> GetAllUserDetailsByName(int pageIndex, int pageSize, string? name);
        Task<UserDetailResponseDTO> GetUserDetailByUserId(int userId);
    }
}
