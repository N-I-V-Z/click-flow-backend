using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum CampaignParticipationStatus
	{
		[EnumMember(Value = "Chờ xử lý")]
		Pending,
		[EnumMember(Value = "Đã tham gia")]
		Participated,
		[EnumMember(Value = "Bị từ chối")]
		Rejected
	}
}
