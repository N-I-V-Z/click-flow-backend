namespace ClickFlow.DAL.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set;}
        public UserPlan? UserPlan { get; set; }
		public ICollection<Report>? Reports { get; set; }
		public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<CampaignParticipation>? CampaignParticipations { get; set; }
        public ICollection<CoursePublisher>? CoursePublishers { get; set; }
	}
}
