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
        [Required(ErrorMessage = "Tên không được để trống.")]
        public string FullName { get; set; }
        [IdentityCardValid]
        public string? IdentityCard { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int? ImageId { get; set; }
    }
}
