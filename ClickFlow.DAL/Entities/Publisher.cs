namespace ClickFlow.DAL.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public string PaymentInfo { get; set; }
        public User? User { get; set;}
		public ICollection<Reports>? Reports { get; set; }
		public ICollection<Feedback>? Feedbacks { get; set; }
		public ICollection<Traffic>? Traffics { get; set; }
		public ICollection<ClosedTraffics>? ClosedTraffics { get; set; }
	}
}
