using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;


namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
	public class CampaignCreateDTO
	{
		[Required(ErrorMessage = "Tên chiến dịch không được để trống.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Mô tả không được để trống.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "URL gốc không được để trống.")]
		[Url(ErrorMessage = "URL gốc không hợp lệ.")]
		public string OriginURL { get; set; }

		[Required(ErrorMessage = "Ngân sách không được để trống.")]
		[Range(1, int.MaxValue, ErrorMessage = "Ngân sách phải là số dương.")]
		public int Budget { get; set; }

		[Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
		[RegularExpression(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$",
		 ErrorMessage = "Ngày bắt đầu phải có định dạng dd/mm/yyyy.")]
		public string StartDate { get; set; }

		[Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
		[RegularExpression(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$",
		 ErrorMessage = "Ngày kết thúc phải có định dạng dd/mm/yyyy.")]
		public string EndDate { get; set; }

		[Required(ErrorMessage = "Loại thanh toán không được để trống.")]
		public TypePay TypePay { get; set; }

		[Required(ErrorMessage = "Loại chiến dịch không được để trống.")]
		public Industry TypeCampaign { get; set; }

		//[EitherCommissionOrPercents]
		//public int? Commission { get; set; }

		//[EitherCommissionOrPercents]
		//public int? Percents { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Commission phải là số không âm.")]
		public int? Commission { get; set; }
		[Range(0, 100, ErrorMessage = "Percents phải nằm trong khoảng 0 đến 100.")]
		public int? Percents { get; set; }
		public string Image { get; set; }
	}
}
