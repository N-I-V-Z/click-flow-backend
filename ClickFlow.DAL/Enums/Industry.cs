using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
	public enum Industry
	{
		[EnumMember(Value = "Thực phẩm và đồ uống")]
		FoodAndBeverage,
		[EnumMember(Value = "Du lịch")]
		Tourism,
		[EnumMember(Value = "Giáo dục")]
		Education,
		[EnumMember(Value = "Khác")]
		Other
	}
}
