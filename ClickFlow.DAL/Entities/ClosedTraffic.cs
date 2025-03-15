namespace ClickFlow.DAL.Entities
{
	public class ClosedTraffic
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
        public int? CampaignParticipationId { get; set; }
        public CampaignParticipation? CampaignParticipation { get; set; }
    }
}
