using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.CampaignParticipationDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ICampaignService
	{
        Task<BaseResponse> CreateCampaign(CampaignCreateDTO dto, int userId);
        Task<BaseResponse> UpdateCampaign(CampaignUpdateDTO dto);
        Task<BaseResponse> UpdateCampaignStatus(CampaignUpdateStatusDTO dto);
        Task<BaseResponse> DeleteCampaign(int id, int userId);

        Task<PaginatedList<CampaignResponseDTO>> GetAllCampaigns(int pageIndex, int pageSize);
        Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByStatus(CampaignStatus? status, int pageIndex, int pageSize);
        Task<PaginatedList<CampaignResponseDTO>> GetCampaignsExceptFromPending(int pageIndex, int pageSize);
        Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByAdvertiserId(int advertiserId, CampaignStatus? status,int pageIndex, int pageSize);
        Task<PaginatedList<CampaignResponseForPublisherDTO>> GetAllCampaignForPublisher(int publisherId, int pageIndex, int pageSize);
        Task<PaginatedList<CampaignResponseDTO>> GetCampaignsJoinedByPublisher(int publisherId, int pageIndex, int pageSize);
        Task<PaginatedList<CampaignParticipationResponseDTO>> GetPublisherPaticipationByStatusForAdvertiser(int advertiserId, CampaignParticipationStatus? campaignParticipationStatus, int pageIndex, int pageSize);
        Task<PaginatedList<CampaignResponseDTO>> GetSimilarCampaignsByTypeCampaign(int campaignId, int pageIndex, int pageSize);
        Task<BaseResponse> RegisterForCampaign(CampaignParticipationCreateDTO dto, int userId);
        Task<CampaignResponseDTO> GetCampaignById(int id);
        Task<BaseResponse> ValidateCampaignForTraffic(int campaignId);
        Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByStatuses(List<CampaignStatus>? statuses, int pageIndex, int pageSize);
        Task UpdateCampaignBudgetAsync(int campaignId, int revenue);
        Task CheckAndStopExpiredCampaigns();
        Task CheckAndStopCampaignIfBudgetExceededAsync(int campaignId);

    }
}
