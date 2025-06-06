using ClickFlow.BLL.DTOs.MessageDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponseDTO> SendMessageAsync(int senderId, MessageSendDTO dto);
        Task<List<MessageResponseDTO>> GetMessagesAsync(int conversationId);
        Task MarkMessagesAsReadAsync(int conversationId, int readerId);
    }
}
