using ClickFlow.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.PostDTOs
{
    public class PostCreateDTO
    {
        public string Tittle {  get; set; }
        public string Content { get; set; }
        public Topic Topic { get; set; }
    }
}
