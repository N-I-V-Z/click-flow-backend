using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.PostDTOs
{
	public class PostResponseDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public Topic Topic { get; set; }
		public DateTime CreatedAt { get; set; }
		public int LikeCount { get; set; }
		public int FeedbackNumber { get; set; }
		public ApplicationUserResponseDTO Author { get; set; }
		public bool IsLikedByCurrentUser { get; set; }
	}
}
