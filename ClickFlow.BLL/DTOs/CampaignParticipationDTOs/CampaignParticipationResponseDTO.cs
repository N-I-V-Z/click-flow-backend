using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.CampaignParticipationDTOs
{
	public class CampaignParticipationResponseDTO
	{
		public int Id { get; set; }
		public CampaignParticipationStatus Status { get; set; }
		public string ShortLink { get; set; }
		public DateTime CreateAt { get; set; }
		public int PublisherId { get; set; }
		public PublisherResponseDTO Publisher { get; set; }
		public int CampaignId { get; set; }
		public CampaignResponseDTO Campaign { get; set; }
		public int TotalCampaigns { get; set; }
		public int DailyTraffic { get; set; } = 0;
	}
}
