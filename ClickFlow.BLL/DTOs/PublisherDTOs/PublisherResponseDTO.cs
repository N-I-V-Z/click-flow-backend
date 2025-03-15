using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.DTOs.PublisherDTOs
{
    public class PublisherResponseDTO
    {
        public int Id { get; set; }
        public ApplicationUserResponseDTO? ApplicationUser { get; set; }
    }
}
