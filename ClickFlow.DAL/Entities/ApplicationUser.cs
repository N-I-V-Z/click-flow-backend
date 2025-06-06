using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

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

        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<CoursePublisher>? CoursePublishers { get; set; }
        public ICollection<Conversation> ConversationsAsUser1 { get; set; }
        public ICollection<Conversation> ConversationsAsUser2 { get; set; }
        [NotMapped]
        public IEnumerable<Conversation> Conversations =>
            (ConversationsAsUser1 ?? Enumerable.Empty<Conversation>())
                .Concat(ConversationsAsUser2 ?? Enumerable.Empty<Conversation>());
        public ICollection<Message>? Messages { get; set; }
    }
}
