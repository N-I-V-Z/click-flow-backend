using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum Gender
    {
		[EnumMember(Value = "Nam")]
		Male,
		[EnumMember(Value = "Nữ")]
		Female,
		[EnumMember(Value = "Khác")]
		Other
	}
}
