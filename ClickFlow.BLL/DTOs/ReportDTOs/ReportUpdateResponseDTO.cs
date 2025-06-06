using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ReportDTOs
{
    public class ReportUpdateResponseDTO
    {
        [Required(ErrorMessage = "Response không được để trống.")]
        public string Response { get; set; }
    }
}
