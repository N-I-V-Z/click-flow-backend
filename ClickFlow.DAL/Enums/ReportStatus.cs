using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
    public enum ReportStatus
    {
        [EnumMember(Value = "Đang chờ")]
        Pending,
        [EnumMember(Value = "Đang xử lý")]
        Processing,
        [EnumMember(Value = "Đã hủy")]
        Canceled,
        [EnumMember(Value = "Đã phê duyệt")]
        Approved,
        [EnumMember(Value = "Đã bị từ chối")]
        Rejected
    }
}
