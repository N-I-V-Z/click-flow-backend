using ClickFlow.BLL.DTOs.TransactionDTOs;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ITransactionService
	{
		Task<TransactionViewDTO> CreateTransactionAsync(TransactionCreateDTO dto);
		Task<TransactionViewDTO> UpdateStatusTransactionAsync(int id, TransactionUpdateStatusDTO dto);
		Task<IEnumerable<TransactionViewDTO>> GetAllTransactionsByWalletIdAsync(int walletId);
	}
}
