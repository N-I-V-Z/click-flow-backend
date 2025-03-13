using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.AdvertiserDTOs
{
    public class AdvertiserResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string IntroductionWebsite { get; set; }
        public int StaffSize { get; set; }
        public Industry Industry { get; set; }
    }
}
