namespace ClickFlow.DAL.Entities
{
	public class UserPlan
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public Publisher User { get; set; }

		public int PlanId { get; set; }
		public Plan Plan { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime? ExpirationDate { get; set; }

		public int CurrentClicks { get; set; }
		public int CurrentConversions { get; set; }
		public int CurrentCampaigns { get; set; }
	}
}
