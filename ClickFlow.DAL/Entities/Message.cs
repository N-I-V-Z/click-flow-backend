using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public int SenderId { get; set; }
        public ApplicationUser Sender { get; set; }

        public string? Text { get; set; }

        public MessageType Type { get; set; }

        public string? FileUrl { get; set; }

        public DateTime? SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
