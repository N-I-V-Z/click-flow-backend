using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<ApplicationUserResponseDTO>> GetUsersByRoleAsync(Role role, int pageIndex, int pageSize);
    }
}
