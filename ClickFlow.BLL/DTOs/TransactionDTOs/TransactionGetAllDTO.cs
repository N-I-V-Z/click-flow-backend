using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionGetAllDTO : PagingRequestDTO
	{
		public bool? Status { get; set; } = null;
		public TransactionType? TransactionType { get; set; }
	}
}
