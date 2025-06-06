using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.CourseDTOs
{
    public class CourseRateDTO
    {
        [Range(1, 5, ErrorMessage = "Rate trong khoảng từ 1 đến 5")]
        public int Rate { get; set; }
    }
}
