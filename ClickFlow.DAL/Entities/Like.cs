using System.ComponentModel.DataAnnotations;

namespace ClickFlow.DAL.Entities
{
	public class Like
	{
		public int Id { get; set; }
		public int PostId { get; set; }
		public Post Post { get; set; }
		public int UserId { get; set; }
		public ApplicationUser User { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsDeleted { get; set; }
	}
} 