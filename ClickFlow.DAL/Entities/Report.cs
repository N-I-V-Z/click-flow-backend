using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreateAt { get; set; }
        public string Response { get; set; }
        public string EvidenceURL { get; set; }
        public int? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }
        public int? AdvertiserId { get; set; }
        public Advertiser? Advertiser { get; set; }
        public int? CampaignId { get; set; }
        public Campaign? Campaign { get; set; }
    }
}
