namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionGetAllDTO : TransactionGetByUserIdDTO
	{
		public bool? Status { get; set; } = null;
	}
}
