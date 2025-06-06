using Microsoft.AspNetCore.Http;

namespace ClickFlow.BLL.DTOs.UploadImageDTOs
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; }
    }
}
