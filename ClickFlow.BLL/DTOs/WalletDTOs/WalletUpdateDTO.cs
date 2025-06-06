using System.ComponentModel;

namespace ClickFlow.BLL.DTOs.WalletDTOs
{
	public class WalletUpdateDTO
	{
		[DefaultValue(0)]
		public int Balance { get; set; }
	}
}
