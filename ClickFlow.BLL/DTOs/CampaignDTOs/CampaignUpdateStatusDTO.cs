using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.CampaignDTOs
{
    public class CampaignUpdateStatusDTO
    {
        [Required(ErrorMessage = "ID không được để trống.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        public CampaignStatus? Status { get; set; }

        public bool IsStatusValid()
        {
            return Enum.IsDefined(typeof(CampaignStatus), Status);
        }
    }
}

