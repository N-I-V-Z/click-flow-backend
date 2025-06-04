using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.DTOs.TrafficDTOs
{
	public class TrafficResponseDTO
	{
		public int Id { get; set; }
		public string ClickId { get; set; }
		public bool IsClosed { get; set; }
		public bool IsValid { get; set; }
		public string IpAddress { get; set; }
		public string DeviceType { get; set; }
		public string Browser { get; set; }
		public string ReferrerURL { get; set; }
		public DateTime Timestamp { get; set; }
		public int? CampaignParticipationId { get; set; }
		public CampaignParticipation? CampaignParticipation { get; set; }
		public ICollection<Conversion>? Conversions { get; set; }
	}
}
