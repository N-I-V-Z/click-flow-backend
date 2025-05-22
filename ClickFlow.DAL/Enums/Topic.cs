using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Enums
{
    public enum Topic
    {
        [EnumMember(Value = "Hỏi đáp")]
        QA,
		[EnumMember(Value = "Mẹo")]
        Tips,
		[EnumMember(Value = "Khác")]
        Other
    }
}
