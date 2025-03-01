using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum CampaignStatus
	{
		[EnumMember(Value = "Chờ xử lý")]
		Pending,
		[EnumMember(Value = "Đã duyệt")]
		Approved,
		[EnumMember(Value = "Đang hoạt động")]
		Activing,
		[EnumMember(Value = "Tạm dừng")]
		Paused,
		[EnumMember(Value = "Đã hủy")]
		Canceled,
		[EnumMember(Value = "Hoàn thành")]
		Completed
	}
}
