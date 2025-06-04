using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.ReportDTOs
{
	public class ReportCreateDTO
	{
		[Required(ErrorMessage = "Reason không được để trống.")]
		[MaxLength(255, ErrorMessage = "Reason không được quá 255 ký tự.")]
		public string Reason { get; set; }

		[Required(ErrorMessage = "EvidenceURL không được để trống.")]
		[MaxLength(255, ErrorMessage = "EvidenceURL không được quá 255 ký tự.")]
		public string EvidenceURL { get; set; }
		public int? PublisherId { get; set; }
		public int? CampaignId { get; set; }
	}
}
