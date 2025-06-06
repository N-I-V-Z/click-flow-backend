namespace ClickFlow.DAL.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int StarRate { get; set; }
        public DateTime Timestamp { get; set; }
        public int? CampaignId { get; set; }
        public Campaign? Campaign { get; set; }
        public int? FeedbackerId { get; set; }
        public Publisher? Feedbacker { get; set; }
    }
}
