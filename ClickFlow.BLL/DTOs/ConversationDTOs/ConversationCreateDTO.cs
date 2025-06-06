using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ConversationDTOs
{
    public class ConversationCreateDTO
    {
        [Required(ErrorMessage = "TargetUserId không được để trống.")]
        public int TargetUserId { get; set; }
    }
}
