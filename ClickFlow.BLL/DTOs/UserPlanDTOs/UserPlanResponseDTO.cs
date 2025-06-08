using ClickFlow.BLL.DTOs.PlanDTOs;
using ClickFlow.BLL.DTOs.PublisherDTOs;

namespace ClickFlow.BLL.DTOs.UserPlanDTOs
{
	public class UserPlanResponseDTO
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public PublisherResponseDTO User { get; set; }

		public int PlanId { get; set; }
		public PlanResponseDTO Plan { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime? ExpirationDate { get; set; }

		public int CurrentClicks { get; set; }
		public int CurrentConversions { get; set; }
		public int CurrentCampaigns { get; set; }
	}
}
