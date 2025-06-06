using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.ReportDTOs
{
    public class ReportGetAllDTO : PagingRequestDTO
    {
        public ReportStatus? Status { get; set; }
    }
}
