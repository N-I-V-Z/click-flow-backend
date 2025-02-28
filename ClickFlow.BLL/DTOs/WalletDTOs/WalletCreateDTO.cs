using ClickFlow.DAL.Entities;
using System.ComponentModel;

namespace ClickFlow.BLL.DTOs.WalletDTOs
{
	public class WalletCreateDTO
	{
		[DefaultValue(0)]
		public int Balance { get; set; }
	}
}
