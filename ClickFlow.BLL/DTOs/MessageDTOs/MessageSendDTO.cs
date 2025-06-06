using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.MessageDTOs
{
    public class MessageSendDTO
    {
        [Required(ErrorMessage = "ConversationId không được để trống.")]
        public int ConversationId { get; set; }
        public string? Text { get; set; }
        [Required(ErrorMessage = "MessageType không được để trống.")]
        public MessageType Type { get; set; }
        public IFormFile? File { get; set; }

    }
}
