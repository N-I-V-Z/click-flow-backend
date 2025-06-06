using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.UserPlanDTOs
{
    public class UserPlanAssignDTO
    {
        [Required(ErrorMessage = "PlanId không dược để trống.")]
        public int PlanId { get; set; }
    }
}
