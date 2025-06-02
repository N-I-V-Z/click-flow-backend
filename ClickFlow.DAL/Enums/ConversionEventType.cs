using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum ConversionEventType
	{
		[EnumMember(Value = "Sale")]
		Sale, // Mua hàng thành công (CPS)
		[EnumMember(Value = "Lead")]
		Lead, // Điền form đăng ký (CPA)
		[EnumMember(Value = "Signup")]
		Signup, // Tạo tài khoản (CPA)
		[EnumMember(Value = "Install")]
		Install, // Cài đặt app (CPA)
		[EnumMember(Value = "FirstOrder")]
		FirstOrder, // Đơn hàng đầu tiên (CPA/CPS)
		[EnumMember(Value = "Qualified")]
		Qualified, // Đơn hàng được duyệt (approved) (CPA)
		[EnumMember(Value = "Conversion")]
		Conversion // Chuyển đổi chung (generic) Fallback
	}
}
