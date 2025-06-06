using ClickFlow.BLL.DTOs.ReportDTOs;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IReportService
	{
		Task<ReportResponseDTO> CreateReportAsync(int advertiserId, ReportCreateDTO dto);
		Task<PaginatedList<ReportResponseDTO>> GetAllAsync(ReportGetAllDTO dto);
		Task<ReportResponseDTO> GetByIdAsync(int id);
		Task<ReportResponseDTO> UpdateStatusReportAsync(int id, ReportStatus status);
		Task<ReportResponseDTO> UpdateResponseReportAsync(int id, string response);
	}
}
