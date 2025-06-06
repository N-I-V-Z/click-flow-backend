using ClickFlow.DAL.Enums;

namespace ClickFlow.BLL.DTOs.PostDTOs
{
    public class PostUpdateDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Topic Topic { get; set; }
    }
}
