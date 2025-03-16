using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.DTOs.PublisherDTOs
{
    public class PublisherViewDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ApplicationUserResponseDTO? ApplicationUser { get; set; }
    }
}
