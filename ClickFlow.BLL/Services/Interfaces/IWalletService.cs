using ClickFlow.BLL.DTOs.WalletDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IWalletService
	{
		Task<WalletResponseDTO> CreateWalletAsync(int userId, WalletCreateDTO dto);
		Task<WalletResponseDTO> UpdateWalletAsync(int id, WalletUpdateDTO dto);
		Task<WalletResponseDTO> GetWalletByUserIdAsync(int id);
	}
}
