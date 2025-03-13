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
		public int? AdvertiserId { get; set; }
		public int? CampaignId { get; set; }
	}
}
