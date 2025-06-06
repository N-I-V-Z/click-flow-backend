using ClickFlow.BLL.DTOs.PagingDTOs;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ConversionDTOs
{
    public class ConversionGetByClickIdDTO : PagingRequestDTO
    {
        [Required(ErrorMessage = "ClickId không được để trống.")]
        public string ClickId { get; set; }
    }
}
