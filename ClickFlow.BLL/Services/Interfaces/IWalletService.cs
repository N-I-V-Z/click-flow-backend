using ClickFlow.BLL.DTOs.WalletDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IWalletService
	{
		Task<WalletViewDTO> CreateWalletAsync(WalletCreateDTO dto);
		Task<WalletViewDTO> UpdateWalletAsync(int id, WalletUpdateDTO dto);
		//Task<WalletViewDTO> GetWalletByUserIdAsync(int id);
	}
}
