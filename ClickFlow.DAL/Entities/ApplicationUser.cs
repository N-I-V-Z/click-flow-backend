using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace ClickFlow.DAL.Entities
{
	public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }
        public UserDetail UserDetail { get; set; }
        public Advertiser? Advertiser { get; set; }
        public Publisher? Publisher { get; set; }
        public Wallet? Wallet { get; set; }
        public ICollection<CoursePublisher>? CoursePublishers { get; set; }
    }
}
