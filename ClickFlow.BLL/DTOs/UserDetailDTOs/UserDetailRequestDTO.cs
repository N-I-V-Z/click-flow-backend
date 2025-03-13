using ClickFlow.BLL.Helpers.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.UserDetailDTOs
{
    public class UserDetailRequestDTO
    {
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string AvatarURL { get; set; }
        public string Address { get; set; }
    }
}
