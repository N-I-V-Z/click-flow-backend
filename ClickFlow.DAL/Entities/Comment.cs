namespace ClickFlow.DAL.Entities
{
	public class Comment
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int AuthorId { get; set; }
		public ApplicationUser Author { get; set; }
		public int PostId { get; set; }
		public int? ParentCommentId { get; set; }

		public Post Post { get; set; }

		public Comment ParentComment { get; set; }
		public bool IsDeleted { get; set; }
		public ICollection<Comment> Replies { get; set; }
	}
}
