using System.ComponentModel.DataAnnotations;
namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
    public class TransactionUpdateStatusDTO
    {
        [Required(ErrorMessage = "Status không được để trống.")]
        public bool? Status { get; set; }
    }
}
