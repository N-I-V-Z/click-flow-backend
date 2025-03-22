using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.CampaignParticipationDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : BaseAPIController
    {
        private readonly ICampaignService _campaignService;

        public CampaignsController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }


        [HttpGet]
        [Route("get-all-campaigns/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllCampaigns([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetAllCampaigns(pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }

        [HttpGet]
        [Route("get-campaigns-except-from-pending/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsExceptFromPending([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetCampaignsExceptFromPending(pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }


        [HttpGet]
        [Route("get-campaigns-by-status/{status}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsByStatus([FromRoute] CampaignStatus? status, [FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetCampaignsByStatus(status, pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
        [HttpGet]
        [Route("get-campaigns-by-statuses/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsByStatuses([FromQuery] List<CampaignStatus>? statuses, [FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetCampaignsByStatuses(statuses, pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }

        [HttpGet]
        [Route("get-publisher-paticipation-by-status-for-advertiser/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetPublisherPaticipationByStatusForAdvertiser([FromQuery] CampaignParticipationStatus? campaignParticipationStatus, [FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }
                var data = await _campaignService.GetPublisherPaticipationByStatusForAdvertiser(UserId, campaignParticipationStatus, pageIndex, pageSize);
                var response = new PagingDTO<CampaignParticipationResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }

        [HttpGet]
        [Route("get-campaigns-by-advertiser/{advertiserId}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsByAdvertiserId([FromRoute] int advertiserId, [FromQuery] CampaignStatus? status, [FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetCampaignsByAdvertiserId(advertiserId, status, pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }


        [HttpGet]
        [Route("get-campaigns-joined-by-publisher/{publisherId}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsJoinedByPublisher([FromRoute] int publisherId, [FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetCampaignsJoinedByPublisher(publisherId, pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }

        [HttpGet]
        [Route("get-all-campaign-for-publisher/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetAllCampaignForPublisher([FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageIndex <= 0)
                {
                    return GetError("Page Index phải là số nguyên dương.");
                }

                if (pageSize <= 0)
                {
                    return GetError("Page Size phải là số nguyên dương.");
                }

                var data = await _campaignService.GetAllCampaignForPublisher(UserId, pageIndex, pageSize);
                var response = new PagingDTO<CampaignResponseForPublisherDTO>(data);
                if (response == null) return GetError();
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }

        [HttpGet]
        [Route("get-campaign-by-id/{id}")]
        public async Task<IActionResult> GetCampaignById(int id)
        {
            var response = await _campaignService.GetCampaignById(id);
            if (response == null) return GetError("Chiến dịch không tồn tại.");
            return GetSuccess(response);
        }


        [HttpPost]
        [Route("create-campaign")]
        public async Task<IActionResult> CreateCampaign(CampaignCreateDTO dto)
        {
            if (!ModelState.IsValid) return ModelInvalid();
            var response = await _campaignService.CreateCampaign(dto, UserId);
            if (!response.IsSuccess) return SaveError(response);
            return SaveSuccess(response);
        }


        [HttpPut]
        [Route(("update-campaign"))]
        public async Task<IActionResult> UpdateCampaign([FromBody]  CampaignUpdateDTO dto)
        {
            if (!ModelState.IsValid) return ModelInvalid();
            var response = await _campaignService.UpdateCampaign(dto);
            if (!response.IsSuccess) return SaveError(response);
            return SaveSuccess(response);
        }

        [HttpPut]
        [Route("update-campaign-status")]
        public async Task<IActionResult> UpdateCampaignStatus(CampaignUpdateStatusDTO dto)
        {
            if (!ModelState.IsValid) return ModelInvalid();
            if (!dto.IsStatusValid())
            {
                ModelState.AddModelError("Status", "Trạng thái không hợp lệ.");
                return ModelInvalid();
            }
            var response = await _campaignService.UpdateCampaignStatus(dto);
            if (!response.IsSuccess) return SaveError(response);
            return SaveSuccess(response);
        }


        [HttpDelete]
        [Route("delete-campaign/{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            var response = await _campaignService.DeleteCampaign(id, UserId);
            if (!response.IsSuccess) return SaveError(response);
            return SaveSuccess(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterForCampaign([FromBody] CampaignParticipationCreateDTO dto)
        {
            var response = await _campaignService.RegisterForCampaign(dto, UserId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }



    }
}
