using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.ConversionDTOs
{
	public class ConversionGetAllDTO : PagingRequestDTO
	{
		public ConversionEventType? EventType { get; set; }
		public string ClickId { get; set; }
	}
}
