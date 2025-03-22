using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OriginURL { get; set; }
        public int Budget { get; set; }
        public int? RemainingBudget { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TypePay TypePay { get; set; }
        public Industry TypeCampaign { get; set; }
        public CampaignStatus Status { get; set; }
        public int? Commission { get; set; }
        public int? Percents { get; set; }
        public string Image { get; set; }
        public double? AverageStarRate { get; set; }
        public bool IsDeleted { get; set; }
		public int? AdvertiserId { get; set; }
        public Advertiser? Advertiser { get; set; }
		public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<Report>? Reports { get; set; }
        public ICollection<CampaignParticipation>? CampaignParticipations { get; set; }
    }
}
