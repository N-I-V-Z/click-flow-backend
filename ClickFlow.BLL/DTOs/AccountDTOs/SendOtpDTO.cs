using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.AccountDTOs
{
    public class SendOtpDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; } = null!;
    }
}
