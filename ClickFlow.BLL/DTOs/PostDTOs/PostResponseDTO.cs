using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.PostDTOs
{
    public class PostResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Topic Topic { get; set; }
        public DateTime CreatedAt { get; set; }
        public int View { get; set; }
        public int FeedbackNumber { get; set; }
        public string AuthorName { get; set; }
    }
}
