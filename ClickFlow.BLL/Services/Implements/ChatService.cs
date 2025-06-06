using ClickFlow.BLL.DTOs.ConversationDTOs;
using ClickFlow.BLL.DTOs.MessageDTOs;
using ClickFlow.BLL.Services.Interfaces;

namespace ClickFlow.BLL.Services.Implements
{
    public class ChatService : IChatService
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly IPusherService _pusherService;

        public ChatService(IConversationService conversationService, IMessageService messageService, IPusherService pusherService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _pusherService = pusherService;
        }
        public Task<List<MessageResponseDTO>> GetMessagesAsync(int conversationId) => _messageService.GetMessagesAsync(conversationId);

        public Task<ConversationResponseDTO> GetOrCreateConversationAsync(int userId1, int userId2) => _conversationService.GetOrCreateAsync(userId1, userId2);

        public Task<List<ConversationResponseListDTO>> GetUserConversationsAsync(int userId) => _conversationService.GetUserConversationsAsync(userId);

        public async Task MarkMessagesAsReadAsync(int conversationId, int readerId)
        {
            await _messageService.MarkMessagesAsReadAsync(conversationId, readerId);

            await _pusherService.TriggerMessageAsync(
                channel: $"private-conversation-{conversationId}",
                eventName: "messages-read",
                data: new
                {
                    readerId,
                    conversationId
                });
        }

        public async Task<MessageResponseDTO> SendMessageAsync(int senderId, MessageSendDTO dto)
        {
            var message = await _messageService.SendMessageAsync(senderId, dto);

            await _pusherService.TriggerMessageAsync(
                channel: $"private-conversation-{dto.ConversationId}",
                eventName: "new-message",
                data: new
                {
                    id = message.Id,
                    text = message.Text,
                    senderId = message.SenderId,
                    sentAt = message.SentAt,
                    fileUrl = message.FileUrl,
                    type = message.Type.ToString()
                });

            return message;
        }
    }
}
