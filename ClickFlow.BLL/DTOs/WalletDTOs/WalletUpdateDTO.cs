using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.WalletDTOs
{
	public class WalletUpdateDTO
	{
		[DefaultValue(0)]
		public int Balance { get; set; }
	}
}
