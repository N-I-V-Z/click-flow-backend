using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.PostDTOs
{
    public class PostSearchDTO
    {
        public string? Keyword { get; set; }
        public Topic? Topic { get; set; }
        public int? AuthorId { get; set; }
    }
} 