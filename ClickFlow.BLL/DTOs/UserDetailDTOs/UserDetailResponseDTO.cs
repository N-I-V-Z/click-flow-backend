namespace ClickFlow.BLL.DTOs.UserDetailDTOs
{
	public class UserDetailResponseDTO
	{
		public int Id { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string Gender { get; set; }
		public string AvatarURL { get; set; }
		public string Address { get; set; }
		public int ApplicationUserId { get; set; }
		public string FullName { get; set; }
	}
}
