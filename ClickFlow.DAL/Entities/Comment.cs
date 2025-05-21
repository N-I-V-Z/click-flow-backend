using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorId { get; set; } 
        public ApplicationUser Author { get; set; }
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }

        public Post Post { get; set; }

        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}
