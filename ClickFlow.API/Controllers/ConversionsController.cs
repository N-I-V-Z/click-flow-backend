using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.ConversionDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversionsController : BaseAPIController
    {
        private readonly IConversionService _conversionService;
        private readonly IUserPlanService _userPlanService;

        public ConversionsController(IConversionService conversionService, IUserPlanService userPlanService)
        {
            _conversionService = conversionService;
            _userPlanService = userPlanService;
        }

        [HttpPost("postback")]
        public async Task<IActionResult> Create([FromBody] ConversionCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                // 1) Kiểm tra quota conversion trước
                //    Cần biết PublisherId từ dto.ClickId → extension: conversionService trả publisherId
                var publisher = await _conversionService.GetPublisherIdByClickId(dto.ClickId);
                if (publisher == null)
                    return SaveError("Không tìm thấy Publisher tương ứng với ClickId.");

                var canConvert = await _userPlanService.IncreaseConversionCountAsync(publisher.Id);
                if (!canConvert)
                    return SaveError("Bạn đã hết hạn mức conversion cho gói hiện tại.");

                // 2) Tạo conversion (service sẽ tự cộng tiền vào ví nếu cần)
                var result = await _conversionService.CreateAsync(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return SaveError("Đã xảy ra lỗi khi xử lý conversion.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ConversionGetAllDTO dto)
        {
            try
            {
                var data = await _conversionService.GetAllAsync(dto);
                var response = new PagingDTO<ConversionResponseDTO>(data);

                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return GetError();
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _conversionService.GetByIdAsync(id);
                if (result == null)
                    return GetNotFound($"Conversion with ID {id} not found");

                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return GetError();
            }
        }

        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ConversionUpdateStatusDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var result = await _conversionService.UpdateStatusAsync(id, dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return SaveError();
            }
        }


    }
}
