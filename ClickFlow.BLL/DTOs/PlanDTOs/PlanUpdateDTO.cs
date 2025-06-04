using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.PlanDTOs
{
	public class PlanUpdateDTO
	{
		[Required(ErrorMessage = "Name không được để trống.")]
		public string Name { get; set; }
		[Required(ErrorMessage = "MaxCampaigns không được để trống.")]
		public int MaxCampaigns { get; set; }
		[Required(ErrorMessage = "MaxClicksPerMonth không được để trống.")]
		public int MaxClicksPerMonth { get; set; }
		[Required(ErrorMessage = "MaxConversionsPerMonth không được để trống.")]
		public int MaxConversionsPerMonth { get; set; }
		public string? Description { get; set; }
	}
}
