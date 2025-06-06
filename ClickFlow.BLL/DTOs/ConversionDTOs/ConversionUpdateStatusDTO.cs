using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ConversionDTOs
{
	public class ConversionUpdateStatusDTO
	{
		[Required(ErrorMessage = "Status không được để trống.")]
		public ConversionStatus Status { get; set; }

	}
}
