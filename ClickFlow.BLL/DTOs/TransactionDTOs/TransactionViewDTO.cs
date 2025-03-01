using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionViewDTO
	{
		public int Id { get; set; }
		public int Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public bool? Status { get; set; }
		public int? Balance { get; set; }
		public TransactionType TransactionType { get; set; }
		public int? WalletId { get; set; }
	}
}
