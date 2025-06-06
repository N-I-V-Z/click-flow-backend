using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.AccountDTOs
{
    public class SignOutDTO
    {
        [Required(ErrorMessage = "RefreshToken không được để trống")]
        public string RefreshToken { get; set; } = null!;
    }
}
