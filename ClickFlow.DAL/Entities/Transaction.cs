using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
	public class Transaction
	{
		public int Id { get; set; }
		public int Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public bool? Status { get; set; }
		public int? Balance { get; set; }
		public TransactionType TransactionType { get; set; }
		public int? WalletId { get; set; }
		public Wallet? Wallet { get; set; }
	}
}
