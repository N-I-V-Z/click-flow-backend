using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
    public class CampaignParticipation
    {
        public int Id { get; set; }
        public CampaignParticipationStatus Status { get; set; }
        public string ShortLink { get; set; }
        public DateTime CreateAt { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public ICollection<Traffic>? Traffics { get; set; }
        public ICollection<ClosedTraffic>? ClosedTraffics { get; set; }

    }
}
