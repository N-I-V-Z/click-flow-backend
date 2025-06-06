using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.ConversionDTOs
{
	public class ConversionResponseDTO
	{
		public int Id { get; set; }
		public string ClickId { get; set; }
		public TrafficResponseDTO Click { get; set; }
		public ConversionEventType EventType { get; set; }
		public int? Revenue { get; set; }
		public string OrderId { get; set; }
		public ConversionStatus Status { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
