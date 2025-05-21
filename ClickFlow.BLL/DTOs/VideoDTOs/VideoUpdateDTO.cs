using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.VideoDTOs
{
	public class VideoUpdateDTO
	{
		[Required(ErrorMessage = "Link không được để trống.")]
		public string Link { get; set; }
		[Required(ErrorMessage = "Title không được để trống.")]
		public string Title { get; set; }
	}
}
