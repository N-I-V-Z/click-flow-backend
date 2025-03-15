using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IPublisherService
	{
        Task<PaginatedList<PublisherResponseDTO>> GetAllPublishersAsync(int pageIndex, int pageSize);
    }
}
