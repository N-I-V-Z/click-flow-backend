using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDTO> CreateTransactionAsync(TransactionCreateDTO dto);
        Task<TransactionResponseDTO> UpdateStatusTransactionAsync(int id, TransactionUpdateStatusDTO dto);
        Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsByUserIdAsync(int userId, PagingRequestDTO dto);
        Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsAsync(PagingRequestDTO dto);
    }
}
