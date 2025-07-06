using ClickFlow.BLL.DTOs.PagingDTOs;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionGetAllDTO : PagingRequestDTO
	{
		public bool? Status { get; set; } = null;
	}
}
