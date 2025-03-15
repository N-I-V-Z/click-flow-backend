using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.ReportDTOs
{
	public class ReportViewDTO
	{
		public int Id { get; set; }
		public string Reason { get; set; }
		public ReportStatus Status { get; set; }
		public DateTime CreateAt { get; set; }
		public string Response { get; set; }
		public string EvidenceURL { get; set; }
		public int? PublisherId { get; set; }
        public PublisherResponseDTO? Publisher { get; set; }
        public int? AdvertiserId { get; set; }
        public AdvertiserViewDTO? Advertiser { get; set; }
        public int? CampaignId { get; set; }
        public CampaignResponseDTO? Campaign { get; set; }
    }
}
