using ClickFlow.BLL.DTOs.ApplicationUserDTOs;

namespace ClickFlow.BLL.DTOs.PublisherDTOs
{
	public class PublisherResponseDTO
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public int UserId { get; set; }
		public ApplicationUserResponseDTO? ApplicationUser { get; set; }
	}
}
