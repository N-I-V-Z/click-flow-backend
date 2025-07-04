using System.ComponentModel;

namespace ClickFlow.BLL.DTOs.WalletDTOs
{
	public class WalletCreateDTO
	{
		[DefaultValue(0)]
		public int Balance { get; set; }
		public string? BankCode { get; set; }
		public string? BankName { get; set; }
	}
}
