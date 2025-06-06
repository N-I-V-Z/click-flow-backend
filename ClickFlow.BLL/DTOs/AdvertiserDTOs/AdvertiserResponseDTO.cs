using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.AdvertiserDTOs
{
    public class AdvertiserResponseDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string IntroductionWebsite { get; set; }
        public int StaffSize { get; set; }
        public Industry Industry { get; set; }
        public ApplicationUserResponseDTO ApplicationUser { get; set; }

    }
}
