using ClickFlow.BLL.DTOs.ConversationDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IConversationService
    {
        Task<ConversationResponseDTO> GetOrCreateAsync(int user1Id, int user2Id);
        Task<List<ConversationResponseListDTO>> GetUserConversationsAsync(int userId);
        Task<ConversationResponseDTO> GetConversasionByIdAsync(int conversationId);
    }
}
