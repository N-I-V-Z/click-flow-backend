using ClickFlow.DAL.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionCreateDTO
	{
		[Required(ErrorMessage = "Amount không được để trống.")]
		[Range(1000, int.MaxValue, ErrorMessage = "Amount phải lớn hơn 1000")]
		public int Amount { get; set; }
		[Required(ErrorMessage = "TransactionType không được để trống.")]
		public TransactionType TransactionType { get; set; }
	}
}
