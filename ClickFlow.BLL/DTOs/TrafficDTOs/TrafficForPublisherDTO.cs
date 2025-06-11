using ClickFlow.BLL.DTOs.PagingDTOs;

namespace ClickFlow.BLL.DTOs.TrafficDTOs
{
	public class TrafficForPublisherDTO : PagingRequestDTO
	{
		public int? CampaignId { get; set; }
	}
}
