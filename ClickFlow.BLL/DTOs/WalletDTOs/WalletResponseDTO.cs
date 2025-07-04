namespace ClickFlow.BLL.DTOs.WalletDTOs
{
	public class WalletResponseDTO
	{
		public int Id { get; set; }
		public int Balance { get; set; }
		public int UserId { get; set; }
		public string? BankCode { get; set; }
		public string? BankName { get; set; }
	}
}
