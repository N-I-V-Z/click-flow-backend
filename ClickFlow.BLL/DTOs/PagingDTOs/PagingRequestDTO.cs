namespace ClickFlow.BLL.DTOs.PagingDTOs
{
	public class PagingRequestDTO
	{
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public string? Keyword { get; set; }
	}
}
