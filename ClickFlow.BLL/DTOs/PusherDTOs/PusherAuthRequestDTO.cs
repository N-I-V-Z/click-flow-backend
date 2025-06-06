using System.Text.Json.Serialization;

namespace ClickFlow.BLL.DTOs.PusherDTOs
{
	public class PusherAuthRequestDTO
	{
		public string SocketId { get; set; }
		public string ChannelName { get; set; }
	}
}
