using ClickFlow.BLL.DTOs.FeedbackDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IFeedbackService
	{
		Task<BaseResponse> CreateFeedback(FeedbackCreateDTO dto, int feedbackerId);
		Task<BaseResponse> UpdateFeedback(FeedbackUpdateDTO dto, int feedbackerId);
		Task<BaseResponse> DeleteFeedback(int id);

		Task<PaginatedList<FeedbackResponseDTO>> GetAllFeedbacks(int pageIndex, int pageSize);
		Task<PaginatedList<FeedbackResponseDTO>> GetFeedbacksByCampaignId(int campaignId, int pageIndex, int pageSize);
		Task<PaginatedList<FeedbackResponseDTO>> GetFeedbacksByFeedbackerId(int feedbackerId, int pageIndex, int pageSize);
		Task<FeedbackResponseDTO> GetFeedbackById(int id);

		Task<bool> HasFeedback(int campaignId, int feedbackerId);
	}
}
