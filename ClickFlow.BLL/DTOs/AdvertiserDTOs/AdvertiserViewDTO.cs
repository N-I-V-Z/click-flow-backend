using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.AdvertiserDTOs
{
    public class AdvertiserViewDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string IntroductionWebsite { get; set; }
        public int StaffSize { get; set; }
        public Industry Industry { get; set; }
        public int UserId { get; set; }
        public UserViewDTO? ApplicationUser { get; set; }
    }
}
