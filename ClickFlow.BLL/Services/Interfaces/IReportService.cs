using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.ReportDTOs;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IReportService
	{
		Task<ReportViewDTO> CreateReportAsync(int advertiserId, ReportCreateDTO dto);
		Task<PaginatedList<ReportViewDTO>> GetAllAsync(PagingRequestDTO dto);
		Task<ReportViewDTO> GetByIdAsync(int id);
		Task<PaginatedList<ReportViewDTO>> GetByStatusAsync(ReportsGetByStatusDTO dto);
		Task<ReportViewDTO> UpdateStatusReportAsync(int id, ReportStatus status);
		Task<ReportViewDTO> UpdateResponseReportAsync(int id, string response);
	}
}
