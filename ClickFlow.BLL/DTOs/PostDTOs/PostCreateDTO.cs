using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.PostDTOs
{
    public class PostCreateDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Topic Topic { get; set; }
    }
}
