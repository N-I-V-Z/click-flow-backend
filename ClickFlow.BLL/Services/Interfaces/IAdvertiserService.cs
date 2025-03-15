using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IAdvertiserService
	{
        Task<PaginatedList<AdvertiserResponseDTO>> GetAllAdvertisersAsync(int pageIndex, int pageSize);

    }
}
