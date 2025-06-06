using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.BLL.DTOs.MessageDTOs;

namespace ClickFlow.BLL.DTOs.ConversationDTOs
{
    public class ConversationResponseListDTO
    {
        public int Id { get; set; }
        public int ParnerId { get; set; }
        public ApplicationUserResponseDTO Parner { get; set; }
        public MessageResponseDTO LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }
}
