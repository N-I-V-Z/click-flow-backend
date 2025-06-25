using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
	public class Post
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int AuthorId { get; set; }
		public ApplicationUser Author { get; set; }
		public Topic Topic { get; set; }
		public int LikeCount { get; set; }
		public int FeedbackNumber { get; set; }
		public bool IsDeleted { get; set; }

		public ICollection<Comment> Comments { get; set; }
		public ICollection<Like> Likes { get; set; }
	}
}
