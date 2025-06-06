namespace ClickFlow.BLL.DTOs.PlanDTOs
{
	public class PlanResponseDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public int MaxCampaigns { get; set; }
		public int MaxClicksPerMonth { get; set; }
		public int MaxConversionsPerMonth { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int? DurationDays { get; set; }
	}
}
