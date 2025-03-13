using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ITransactionService
	{
		Task<TransactionViewDTO> CreateTransactionAsync(TransactionCreateDTO dto);
		Task<TransactionViewDTO> UpdateStatusTransactionAsync(int id, TransactionUpdateStatusDTO dto);
		Task<PaginatedList<TransactionViewDTO>> GetAllTransactionsByWalletIdAsync(int walletId, PagingRequestDTO dto);
	}
}
