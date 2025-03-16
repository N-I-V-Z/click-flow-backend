using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IUserService
	{
        Task<PaginatedList<UserViewDTO>> GetUsersByRoleAsync(Role role, int pageIndex, int pageSize);
    }
}
