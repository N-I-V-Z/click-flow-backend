using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum TransactionType
	{
		[EnumMember(Value = "Nạp tiền")]
		Deposit,
		[EnumMember(Value = "Rút tiền")]
		Withdraw,
		[EnumMember(Value = "Thanh toán")]
		Pay,
		[EnumMember(Value = "Được nhận")]
		Received
	}
}
