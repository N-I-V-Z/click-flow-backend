using ClickFlow.BLL.DTOs.ConversationDTOs;
using ClickFlow.BLL.DTOs.MessageDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IChatService
    {
        Task<ConversationResponseDTO> GetOrCreateConversationAsync(int userId1, int userId2);
        Task<MessageResponseDTO> SendMessageAsync(int senderId, MessageSendDTO dto);
        Task<List<MessageResponseDTO>> GetMessagesAsync(int conversationId);
        Task MarkMessagesAsReadAsync(int conversationId, int readerId);
        Task<List<ConversationResponseListDTO>> GetUserConversationsAsync(int userId);
    }
}
