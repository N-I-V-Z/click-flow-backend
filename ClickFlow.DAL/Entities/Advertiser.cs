using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
    public class Advertiser
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public Industry Industry { get; set; }
        public User? User { get; set; }
		public ICollection<Report>? Reports { get; set; }
		public ICollection<Campaign>? Campaigns { get; set; }
	}
}
