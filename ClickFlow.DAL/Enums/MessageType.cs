using System.Runtime.Serialization;

namespace ClickFlow.DAL.Enums
{
    public enum MessageType
    {
        [EnumMember(Value = "Văn bản")]
        Text,
        [EnumMember(Value = "Hình ảnh")]
        Image
    }

}
