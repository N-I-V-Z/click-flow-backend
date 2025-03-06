using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
    public enum Role
    {
		[EnumMember(Value = "Quản trị viên")]
		Admin = 1,
		[EnumMember(Value = "Nhà cung cấp")]
		Advertiser = 2,
		[EnumMember(Value = "Nhà tiếp thị")]
		Publisher = 3,
	}
}
