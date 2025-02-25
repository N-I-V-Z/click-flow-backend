namespace ClickFlow.DAL.Entities
{
    public class Traffic
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int? ConversionCount {  get; set; }
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string ReferrerURL { get; set; }
        public DateTime Timestamp { get; set; }
        public int? CampaignId { get; set; }
        public int? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        public Campaign? Campaign { get; set; }
    }
}
