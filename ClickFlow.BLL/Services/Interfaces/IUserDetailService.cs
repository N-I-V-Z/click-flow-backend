using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.UserDetailDTOs;
using ClickFlow.DAL.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
