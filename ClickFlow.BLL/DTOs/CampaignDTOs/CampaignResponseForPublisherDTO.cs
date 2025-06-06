using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
	public class CampaignResponseForPublisherDTO : CampaignResponseDTO
	{
		public CampaignParticipationStatus? PublisherStatus { get; set; }
	}
}
