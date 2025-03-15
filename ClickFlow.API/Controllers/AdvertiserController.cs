using ClickFlow.BLL.Services.Implements;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertiserController : BaseAPIController
    {
        private readonly IAdvertiserService _advertiserService;
        private readonly IConfiguration _configuration;

        public AdvertiserController(IAdvertiserService advertiserService, IConfiguration configuration)
        {
            _advertiserService = advertiserService;    
            this._configuration = configuration;
        }

        [HttpGet]
        [Route("get-all-advertisers/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllAdvertisers([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                var response = await _advertiserService.GetAllAdvertisersAsync(pageIndex, pageSize);
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
