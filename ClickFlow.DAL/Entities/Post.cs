using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorId { get; set; }  
        public ApplicationUser Author { get; set; }
        public string Topic { get; set; }
        public int View {  get; set; }
        public int FeedbackNumber { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}
