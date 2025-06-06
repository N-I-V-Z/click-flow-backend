using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ConversionDTOs
{
	public class ConversionCreateDTO
	{
		[Required(ErrorMessage = "ClickId không được để trống.")]
		public string ClickId { get; set; }
		[Required(ErrorMessage = "EventType không được để trống.")]
		public ConversionEventType EventType { get; set; }
		public int? Revenue { get; set; }
		public string OrderId { get; set; }

	}
}
