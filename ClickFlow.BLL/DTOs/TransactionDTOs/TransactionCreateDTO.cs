using ClickFlow.DAL.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
    public class TransactionCreateDTO
    {
        [Required(ErrorMessage = "Amount không được để trống.")]
        public int Amount { get; set; }
        [DefaultValue(false)]
        public bool? Status { get; set; }
        [Required(ErrorMessage = "TransactionType không được để trống.")]
        public TransactionType TransactionType { get; set; }
        [Required(ErrorMessage = "WalletId không được để trống.")]
        public int? WalletId { get; set; }
    }
}
