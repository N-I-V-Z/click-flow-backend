using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ITrafficService
	{
		Task<PaginatedList<TrafficViewDTO>> GetAllAsync(PagingRequestDTO dto);
		//Task<PaginatedList<TrafficViewDTO>> GetAllByPublisherIdAsync(int id, PagingRequestDTO dto);
		//Task<PaginatedList<TrafficViewDTO>> GetAllByAdvertiserIdAsync(int id, PagingRequestDTO dto);
		//Task<PaginatedList<TrafficViewDTO>> GetAllByCampaignIdAsync(int id, PagingRequestDTO dto);
		Task<TrafficViewDTO> GetByIdAsync(int id);
		//Task<TrafficViewDTO> CreateAsync(TrafficCreateDTO dto);
	}
}
