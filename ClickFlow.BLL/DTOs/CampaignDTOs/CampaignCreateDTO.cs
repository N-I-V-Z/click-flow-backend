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
        public string OriginURL { get; set; }

        [Required(ErrorMessage = "Ngân sách không được để trống.")]
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

        public int? Commission { get; set; }
        public int? Percents { get; set; }
        public string Image { get; set; }
    }
}
