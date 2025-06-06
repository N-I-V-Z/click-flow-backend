namespace ClickFlow.BLL.DTOs.CommentDTOs
{
	public class CommentCreateDTO
	{
		public string Content { get; set; }
		public int PostId { get; set; }
		public int? ParentCommentId { get; set; }
	}
}
