using ClickFlow.BLL.DTOs.UserPlanDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IUserPlanService
    {
        Task<UserPlanResponseDTO> GetCurrentPlanAsync(int userId);
        Task<UserPlanResponseDTO> AssignPlanToPublisherAsync(int userId, int planId);
        Task<bool> CanAddCampaignAsync(int userId);
        Task<bool> IncreaseClickCountAsync(int userId);
        Task<bool> IncreaseConversionCountAsync(int userId);
    }
}
