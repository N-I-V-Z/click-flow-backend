using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.MessageDTOs
{
	public class MessageMaskAsReadDTO
	{
		[Required(ErrorMessage = "ConversationId không được để trống.")]
		public int ConversationId { get; set; }
	}
}
