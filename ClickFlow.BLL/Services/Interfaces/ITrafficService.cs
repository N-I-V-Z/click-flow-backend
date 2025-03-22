using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ITrafficService
	{
		Task<PaginatedList<TrafficResponseDTO>> GetAllAsync(PagingRequestDTO dto);
		Task<PaginatedList<TrafficResponseDTO>> GetAllByPublisherIdAsync(int id, PagingRequestDTO dto);
		Task<PaginatedList<TrafficResponseDTO>> GetAllByAdvertiserIdAsync(int id, PagingRequestDTO dto);
		Task<PaginatedList<TrafficResponseDTO>> GetAllByCampaignIdAsync(int id, PagingRequestDTO dto);
		Task<TrafficResponseDTO> GetByIdAsync(int id);
		Task<BaseResponse> ValidateTraffic(TrafficCreateDTO dto);
		Task<string> CreateAsync(TrafficCreateDTO dto);
	}
}
