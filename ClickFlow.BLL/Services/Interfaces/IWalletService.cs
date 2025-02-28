using ClickFlow.BLL.DTOs.WalletDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IWalletService
	{
		Task<WalletViewDTO> CreateWallet(WalletCreateDTO dto);
		Task<WalletViewDTO> UpdateWallet(int id, WalletUpdateDTO dto);
	}
}
