using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ITrafficService
	{
		Task<PaginatedList<TrafficResponseDTO>> GetAllAsync(PagingRequestDTO dto);
		Task<PaginatedList<TrafficResponseDTO>> GetAllByPublisherIdAsync(int id, TrafficForPublisherDTO dto);
		Task<PaginatedList<TrafficResponseDTO>> GetAllByAdvertiserIdAsync(int id, PagingRequestDTO dto);
		Task<PaginatedList<TrafficResponseDTO>> GetAllByCampaignIdAsync(int id, PagingRequestDTO dto);
		Task<TrafficResponseDTO> GetByIdAsync(int id);
		Task<BaseResponse> ValidateTraffic(TrafficCreateDTO dto);
		Task<TrafficClickResponseDTO> CreateAsync(TrafficCreateDTO dto, string remoteIp);
		Task TransferTrafficToClosedTraffic();
		Task<int> AverageTrafficInCampaign(int publisherId);
		Task<int> CountAllTrafficByCampaign(int campaignId);
		Task<bool> IsValidTraffic(TrafficCreateDTO dto, string IpAddress);
	}
}
