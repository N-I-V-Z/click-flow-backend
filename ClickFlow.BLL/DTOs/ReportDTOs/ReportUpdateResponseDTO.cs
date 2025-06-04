using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.ReportDTOs
{
	public class ReportUpdateResponseDTO
	{
		[Required(ErrorMessage = "Response không được để trống.")]
		public string Response { get; set; }
	}
}
