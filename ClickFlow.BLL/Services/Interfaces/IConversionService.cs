using ClickFlow.BLL.DTOs.ConversionDTOs;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IConversionService
	{
		Task<PaginatedList<ConversionResponseDTO>> GetAllAsync(ConversionGetAllDTO dto);
		Task<ConversionResponseDTO> GetByIdAsync(int id);
		Task<ConversionResponseDTO> CreateAsync(ConversionCreateDTO dto);
		Task<ConversionResponseDTO> UpdateStatusAsync(int id, ConversionUpdateStatusDTO dto);
	}
}
