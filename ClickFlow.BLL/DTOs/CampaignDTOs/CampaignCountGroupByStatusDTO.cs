namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
	public class CampaignCountGroupByStatusDTO
	{
		public int PendingCount { get; set; }
		public int ApprovedCount { get; set; }
		public int ActivingCount { get; set; }
		public int PausedCount { get; set; }
		public int CanceledCount { get; set; }
		public int CompletedCount { get; set; }
	}
}
