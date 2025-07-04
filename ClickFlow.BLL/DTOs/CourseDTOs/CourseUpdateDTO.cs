using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.CourseDTOs
{
	public class CourseUpdateDTO
	{
		[Required(ErrorMessage = "Title không được để trống.")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Price không được để trống.")]
		[Range(0, 100_000_000, ErrorMessage = "Price trong khoảng từ 0 đến 100,000,000")]
		public int Price { get; set; }
		public string LessonLearned { get; set; }
		public string Content { get; set; }
		public string Description { get; set; }
	}
}
