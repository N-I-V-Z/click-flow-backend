using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionGetByUserIdDTO : PagingRequestDTO
	{
		public TransactionType? TransactionType { get; set; }
	}
}
