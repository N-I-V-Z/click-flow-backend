namespace ClickFlow.DAL.Entities
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int MaxCampaigns { get; set; }
        public int MaxClicksPerMonth { get; set; }
        public int MaxConversionsPerMonth { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? DurationDays { get; set; }

        public ICollection<UserPlan> UserPlans { get; set; } = new List<UserPlan>();
    }


}
