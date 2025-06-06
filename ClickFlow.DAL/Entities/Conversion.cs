using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
	public class Conversion
	{
		public int Id { get; set; }
		public string ClickId { get; set; }
		public Traffic Click { get; set; }
		public ConversionEventType EventType { get; set; }
		public int? Revenue { get; set; }
		public string OrderId { get; set; }
		public ConversionStatus Status { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
