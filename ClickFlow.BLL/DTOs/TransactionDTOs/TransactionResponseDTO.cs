using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionResponseDTO
	{
		public long Id { get; set; }
		public int Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public bool? Status { get; set; }
		public int? Balance { get; set; }
		public TransactionType TransactionType { get; set; }
		public int? WalletId { get; set; }
		public Wallet? Wallet { get; set; }

	}
}
