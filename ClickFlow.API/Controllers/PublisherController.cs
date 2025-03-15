using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : BaseAPIController
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        [Route("get-all-publishers/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllPublishers([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                var response = await _publisherService.GetAllPublishersAsync(pageIndex, pageSize);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút.");
            }
        }
    }
}
