using ClickFlow.BLL.DTOs.ApplicationUserDTOs;

namespace ClickFlow.BLL.DTOs.LikeDTOs
{
	public class LikeResponseDTO
	{
		public int Id { get; set; }
		public int PostId { get; set; }
		public int UserId { get; set; }
		public ApplicationUserResponseDTO User { get; set; }
		public DateTime CreatedAt { get; set; }
	}
} 