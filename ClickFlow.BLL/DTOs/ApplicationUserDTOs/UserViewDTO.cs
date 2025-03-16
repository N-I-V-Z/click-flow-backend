using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.ApplicationUserDTOs
{
    public class UserViewDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }

        public AdvertiserViewDTO? Advertiser { get; set; }

    }
}
