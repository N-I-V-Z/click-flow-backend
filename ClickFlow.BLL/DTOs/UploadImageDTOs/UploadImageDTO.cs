using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.DTOs.UploadImageDTOs
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; }
    }
}
