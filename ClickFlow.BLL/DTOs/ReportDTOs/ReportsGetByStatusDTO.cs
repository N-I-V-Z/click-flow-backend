using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ReportDTOs
{
    public class ReportsGetByStatusDTO : PagingRequestDTO
    {
        [Required(ErrorMessage = "Status không được để trống.")]
        public ReportStatus Status { get; set; }
    }
}
