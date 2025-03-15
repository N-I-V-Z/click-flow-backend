using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.CampaignParticipationDTOs
{
    public class CampaignParticipationNoTrafficResponseDTO
    {
        public int Id { get; set; }
        public CampaignParticipationStatus Status { get; set; }
        public string ShortLink { get; set; }
        public DateTime CreateAt { get; set; }
        public int PublisherId { get; set; }
        public PublisherViewDTO Publisher { get; set; }
        public int CampaignId { get; set; }
        public CampaignResponseDTO Campaign { get; set; }
    }
}
