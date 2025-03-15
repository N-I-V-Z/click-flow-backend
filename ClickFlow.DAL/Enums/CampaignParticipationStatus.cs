using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
