using ClickFlow.BLL.DTOs.ApplicationUserDTOs;

namespace ClickFlow.BLL.DTOs.PublisherDTOs
{
    public class PublisherViewDTO
    {
        public int Id { get; set; }
        public ApplicationUserResponseDTO? ApplicationUser { get; set; }
    }
}
