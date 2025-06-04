using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.PlanDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IPlanService
	{
		Task<PaginatedList<PlanResponseDTO>> GetAllAsync(PlanGetAllDTO dto);
		Task<PlanResponseDTO> GetByIdAsync(int id);
		Task<PlanResponseDTO> CreateAsync(PlanCreateDTO dto);
		Task<PlanResponseDTO> UpdateAsync(int id, PlanUpdateDTO dto);
		Task<bool> DeleteAsync(int id);
	}
}
