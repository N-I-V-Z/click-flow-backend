using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum ConversionStatus
	{
		[EnumMember(Value = "Đang chờ")]
		Pending,
		[EnumMember(Value = "Đã chấp nhận")]
		Approved,
		[EnumMember(Value = "Đã từ chối")]
		Rejected
	}
}
