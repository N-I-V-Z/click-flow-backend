using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.TransactionDTOs
{
	public class TransactionCreateDTO
	{
		[Required(ErrorMessage = "Amount không được để trống.")]
		[Range(2000, int.MaxValue, ErrorMessage = "Amount phải từ 2000 VNĐ trở lên.")]
		public int Amount { get; set; }
		[Required(ErrorMessage = "TransactionType không được để trống.")]
		public TransactionType TransactionType { get; set; }
	}
}
