using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum TypePay
	{
		[EnumMember(Value = "CPC")]
		CPC,
		[EnumMember(Value = "CPA")]
		CPA,
		[EnumMember(Value = "CPS")]
		CPS
	}
}
