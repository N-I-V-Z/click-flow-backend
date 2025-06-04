using ClickFlow.BLL.DTOs.PagingDTOs;

namespace ClickFlow.BLL.DTOs.PlanDTOs
{
	public class PlanGetAllDTO : PagingRequestDTO
	{
		public bool? IsActive { get; set; }
	}
}
