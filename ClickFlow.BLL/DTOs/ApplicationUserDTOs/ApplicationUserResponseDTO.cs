using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.DTOs.UserDetailDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.ApplicationUserDTOs
{
    public class ApplicationUserResponseDTO
    {
        public string FullName { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }
        public string? Email { get; set; }
        public AdvertiserResponseDTO? Advertiser { get; set; }
        public UserDetailResponseDTO? UserDetail { get; set; }
    }
}
