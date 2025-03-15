using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Enums;
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
        public async Task<IActionResult> GetAllCampaigns(int pageIndex, int pageSize)
        {
            var response = await _campaignService.GetAllCampaigns(pageIndex, pageSize);
            return GetSuccess(response);
        }

        [HttpGet]
        [Route("get-campaigns-by-status/{status}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsByStatus(CampaignStatus? status, int pageIndex, int pageSize)
        {
            var response = await _campaignService.GetCampaignsByStatus(status, pageIndex, pageSize);
            return GetSuccess(response);
        }

        [HttpGet]
        [Route("get-campaigns-by-advertiser/{advertiserId}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetCampaignsByAdvertiserId(int advertiserId, CampaignStatus status, int pageIndex, int pageSize)
        {
            var response = await _campaignService.GetCampaignsByAdvertiserId(advertiserId, status, pageIndex, pageSize);
            return GetSuccess(response);
        }


        //[HttpGet]
        //[Route("get-campaigns-joined-by-publisher/{publisherId}/{pageIndex}/{pageSize}")]
        //public async Task<IActionResult> GetCampaignsJoinedByPublisher(int publisherId, int pageIndex, int pageSize)
        //{
        //    var response = await _campaignService.GetCampaignsJoinedByPublisher(publisherId, pageIndex, pageSize);
        //    return GetSuccess(response);
        //}
        //[HttpGet]
        //[Route("get-advertisers-by-publisher/{publisherId}/{pageIndex}/{pageSize}")]
        //public async Task<IActionResult> GetAdvertisersByPublisher(int publisherId, int pageIndex, int pageSize)
        //{
        //    try
        //    {
        //        var response = await _campaignService.GetAdvertisersByPublisher(publisherId, pageIndex, pageSize);
        //        return GetSuccess(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Đã xảy ra lỗi khi lấy danh sách Advertiser.");
        //    }
        //}

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


    }
}
