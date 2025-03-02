using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
    public enum Role
    {
		[EnumMember(Value = "Quản trị viên")]
		Admin,
		[EnumMember(Value = "Nhà cung cấp")]
		Advertiser,
		[EnumMember(Value = "Nhà tiếp thị")]
		Publisher
	}
}
