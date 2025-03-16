using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.TrafficDTOs
{
	public class TrafficCreateDTO
	{
		[Required(ErrorMessage = "IpAddress không được để trống.")]
		[MaxLength(255, ErrorMessage = "IpAddress không quá 255 ký tự.")]
		public string IpAddress { get; set; }
		[Required(ErrorMessage = "DeviceType không được để trống.")]
		[MaxLength(100, ErrorMessage = "DeviceType không quá 100 ký tự.")]
		public string DeviceType { get; set; }
		public string OrderId { get; set; }
		[Required(ErrorMessage = "Browser không được để trống.")]
		[MaxLength(100, ErrorMessage = "Browser không quá 100 ký tự.")]
		public string Browser { get; set; }
		[Required(ErrorMessage = "ReferrerURL không được để trống.")]
		[MaxLength(255, ErrorMessage = "ReferrerURL không quá 255 ký tự.")]
		public string ReferrerURL { get; set; }
		[Required(ErrorMessage = "Timestamp không được để trống.")]
		public DateTime Timestamp { get; set; }
        [Required(ErrorMessage = "CampaignId không được để trống.")]
        public int CampaignId { get; set; }
        [Required(ErrorMessage = "PublisherId không được để trống.")]
        public int PublisherId { get; set; }
	}
}
