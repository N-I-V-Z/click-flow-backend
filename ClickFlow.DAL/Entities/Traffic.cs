namespace ClickFlow.DAL.Entities
{
	public class Traffic
	{
		public int Id { get; set; }
		public string ClickId { get; set; } = Guid.NewGuid().ToString();
		public bool IsClosed { get; set; } = false;
		public bool IsValid { get; set; } = true;
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
