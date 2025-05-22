using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.CommentDTOs
{
    public class CommentCreateDTO
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
