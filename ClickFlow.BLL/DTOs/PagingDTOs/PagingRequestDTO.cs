using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.PagingDTOs
{
	public class PagingRequestDTO
	{
		[Required(ErrorMessage = "PageIndex không được để trống.")]
		public int PageIndex { get; set; }
        [Required(ErrorMessage = "PageSize không được để trống.")]
        public int PageSize { get; set; }
		public string? Keyword { get; set; }
	}
}
