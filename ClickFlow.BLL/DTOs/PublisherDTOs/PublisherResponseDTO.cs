using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.PublisherDTOs
{
    public class PublisherResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
