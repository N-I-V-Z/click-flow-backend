using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
	public class Advertiser
	{
		public int Id { get; set; }
		public string CompanyName { get; set; }
		public string IntroductionWebsite { get; set; }
		public int StaffSize { get; set; }
		public Industry Industry { get; set; }
		public int UserId { get; set; }
		public ApplicationUser? ApplicationUser { get; set; }
		public ICollection<Report>? Reports { get; set; }
		public ICollection<Campaign>? Campaigns { get; set; }
	}
}
