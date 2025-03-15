using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.ApplicationUserDTOs
{
    public class ApplicationUserResponseDTO
    {
        public string FullName { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }
        public int? AdvertiserId { get; set; }
        public int? PublisherId { get; set; }
        public int? WalletId { get; set; }
    }
}
