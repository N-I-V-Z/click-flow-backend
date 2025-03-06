using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.DTOs.TrafficDTOs
{
	public class TrafficViewDTO
	{
		public int Id { get; set; }
		public string IpAddress { get; set; }
		public bool? IsValid { get; set; }
		public int? Revenue { get; set; }
		public string DeviceType { get; set; }
		public string OrderId { get; set; }
		public string Browser { get; set; }
		public string ReferrerURL { get; set; }
		public DateTime Timestamp { get; set; }
		public int? CampaignId { get; set; }
		public int? PublisherId { get; set; }
		public Campaign? Campaign { get; set; }
	}
}
