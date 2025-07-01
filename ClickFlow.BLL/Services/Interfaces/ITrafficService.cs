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
		Task<BaseResponse> ValidateTrafficAsync(TrafficCreateDTO dto);
		Task<TrafficClickResponseDTO> CreateAsync(TrafficCreateDTO dto, string remoteIp);
		Task TransferTrafficToClosedTrafficAsync();
		Task<int> AverageTrafficInCampaignAsync(int publisherId);
		Task<int> CountAllTrafficByCampaignAsync(int campaignId);
		Task<bool> IsValidTrafficAsync(TrafficCreateDTO dto, string IpAddress);
		Task<int> CountTrafficForPublisherAsync(int campaignId, int publisherId);
		Task<int> CountTrafficOfAllActiveCampaignForPublisherAsync(int publisherId);
		Task<List<TrafficBrowserStatisticsDTO>> GetBrowserStatisticsAsync(int publisherId);
		Task<List<TrafficBrowserStatisticsDTO>> GetBrowserStatisticsByCampaignAsync(int campaignId);
		Task<List<TrafficDeviceStatisticsDTO>> GetDeviceStatisticsAsync(int publisherId);
		Task<List<TrafficDeviceStatisticsDTO>> GetDeviceStatisticsByCampaignAsync(int campaignId);
	}
}
