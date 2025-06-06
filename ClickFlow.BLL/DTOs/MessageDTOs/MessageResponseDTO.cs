using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.BLL.DTOs.ConversationDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.MessageDTOs
{
	public class MessageResponseDTO
	{
		public int Id { get; set; }

		public int ConversationId { get; set; }
		public ConversationResponseDTO Conversation { get; set; }

		public int SenderId { get; set; }
		public ApplicationUserResponseDTO Sender { get; set; }

		public string? Text { get; set; }

		public MessageType Type { get; set; }

		public string? FileUrl { get; set; }

		public DateTime? SentAt { get; set; }
		public bool IsRead { get; set; }
	}
}
