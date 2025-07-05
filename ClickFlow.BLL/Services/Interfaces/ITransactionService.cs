using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface ITransactionService
	{
		Task<TransactionResponseDTO> CreateTransactionAsync(int userId, TransactionCreateDTO dto);
		Task<TransactionResponseDTO> UpdateStatusTransactionAsync(long id, TransactionUpdateStatusDTO dto);
		Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsByUserIdAsync(int userId, TransactionGetByUserIdDTO dto);
		Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsAsync(PagingRequestDTO dto);
	}
}
