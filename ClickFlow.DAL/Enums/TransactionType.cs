using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
    public enum TransactionType
    {
        [EnumMember(Value = "Nạp tiền")]
        Deposit,
        [EnumMember(Value = "Rút tiền")]
        Withdraw
    }
}
