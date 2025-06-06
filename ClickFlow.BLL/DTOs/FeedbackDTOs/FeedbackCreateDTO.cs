using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.FeedbackDTOs
{
    public class FeedbackCreateDTO
    {
        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Đánh giá sao không được để trống.")]
        [Range(1, 5, ErrorMessage = "Đánh giá sao phải từ 1 đến 5.")]
        public int StarRate { get; set; }

        [Required(ErrorMessage = "ID chiến dịch không được để trống.")]
        public int CampaignId { get; set; }


    }

    public class FeedbackUpdateDTO
    {
        [Required(ErrorMessage = "ID không được để trống.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Đánh giá sao không được để trống.")]
        [Range(1, 5, ErrorMessage = "Đánh giá sao phải từ 1 đến 5.")]
        public int StarRate { get; set; }
    }
}
