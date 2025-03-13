using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.UserDetailDTOs
{
    public class UserDetailViewDTO
    {
        public int Id { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string AvatarURL { get; set; }
        public string Address { get; set; }
        public int ApplicationUserId { get; set; }
        public string FullName { get; set; }
    }
}
