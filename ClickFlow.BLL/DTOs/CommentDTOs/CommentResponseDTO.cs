using ClickFlow.BLL.DTOs.ApplicationUserDTOs;

namespace ClickFlow.BLL.DTOs.CommentDTOs
{
	public class CommentResponseDTO
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int AuthorId { get; set; }
		public string AuthorName { get; set; }
		public int PostId { get; set; }
		public int? ParentCommentId { get; set; }
		public List<CommentResponseDTO> Replies { get; set; }
		public ApplicationUserResponseDTO AuthorInfo { get; set; }
	}
}
