namespace ClickFlow.DAL.Entities
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int MaxCampaigns { get; set; }
        public int MaxClicksPerMonth { get; set; }
        public int MaxConversionsPerMonth { get; set; }
        public bool AllowAPI { get; set; }
        public bool AllowExportReport { get; set; }
        public bool AllowPostbackAdvanced { get; set; }
    }
}
