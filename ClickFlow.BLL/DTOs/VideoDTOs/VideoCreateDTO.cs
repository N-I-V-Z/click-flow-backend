using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.VideoDTOs
{
	public class VideoCreateDTO
	{
		[Required(ErrorMessage = "Link không được để trống.")]
		public string Link { get; set; }
		[Required(ErrorMessage = "Title không được để trống.")]
		public string Title { get; set; }
		[Required(ErrorMessage = "CourseId không được để trống.")]
		public int CourseId { get; set; }
	}
}
