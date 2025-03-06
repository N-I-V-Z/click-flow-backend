namespace ClickFlow.DAL.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public ApplicationUser? ApplicationUser { get; set;}
		public ICollection<Report>? Reports { get; set; }
		public ICollection<Feedback>? Feedbacks { get; set; }
		public ICollection<Traffic>? Traffics { get; set; }
		public ICollection<ClosedTraffic>? ClosedTraffics { get; set; }
	}
}
