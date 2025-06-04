using System.Runtime.Serialization;

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
