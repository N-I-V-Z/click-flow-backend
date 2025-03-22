﻿using AutoMapper;
using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.CampaignParticipationDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace ClickFlow.BLL.Services.Implements
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdvertiserService _advertiserService;
    
     

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IAdvertiserService advertiserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _advertiserService = advertiserService;
          
        }

        public async Task<BaseResponse> CreateCampaign(CampaignCreateDTO dto, int userId)
        {
            try
            {
              
                await _unitOfWork.BeginTransactionAsync();

               
                var advertiserRepo = _unitOfWork.GetRepo<Advertiser>();


                var advertiser = await _advertiserService.GetAdvertiserByUserIdAsync(userId);

                if (advertiser == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy Advertiser." };
                }
        
                var campaignRepo = _unitOfWork.GetRepo<Campaign>();

                var campaign = _mapper.Map<Campaign>(dto);
         
                campaign.AdvertiserId = advertiser.Id;
                campaign.RemainingBudget = campaign.Budget;
              
                campaign.Status = CampaignStatus.Pending;
         
                await campaignRepo.CreateAsync(campaign);           
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Trả về kết quả thành công
                return new BaseResponse { IsSuccess = true, Message = "Chiến dịch đã được tạo thành công." };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();         
                throw new Exception("Đã xảy ra lỗi khi tạo chiến dịch.", ex);
            }
        }

        public async Task<BaseResponse> UpdateCampaign(CampaignUpdateDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Campaign>();

                var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                    .WithPredicate(x => x.Id == dto.Id)
                    .WithTracking(false)
                    .Build());

                if (campaign == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không tồn tại." };
                }

                _mapper.Map(dto, campaign);
                await repo.UpdateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Chiến dịch đã được cập nhật thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<BaseResponse> UpdateCampaignStatus(CampaignUpdateStatusDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Campaign>();

                var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                    .WithPredicate(x => x.Id == dto.Id)
                    .WithTracking(false)
                    .Build());

                if (campaign == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không tồn tại." };
                }

                if (!dto.IsStatusValid())
                {
                    return new BaseResponse { IsSuccess = false, Message = "Trạng thái không hợp lệ." };
                }

                campaign.Status = dto.Status.Value;
                await repo.UpdateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Trạng thái chiến dịch đã được cập nhật thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<BaseResponse> DeleteCampaign(int id, int userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Campaign>();

                var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                    .WithPredicate(x => x.Id == id)
                    .WithTracking(false)
                    .Build());

                if (campaign == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không tồn tại." };
                }

                //if (campaign.CreatedBy != userId)
                //{
                //    return new BaseResponse { IsSuccess = false, Message = "Bạn không có quyền xóa chiến dịch này." };
                //}

                campaign.IsDeleted = true;
                await repo.UpdateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Chiến dịch đã được xóa thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<PaginatedList<CampaignResponseDTO>> GetAllCampaigns(int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted)
                .WithInclude(x => x.Advertiser.ApplicationUser)   

                
                .Build());

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsExceptFromPending(int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && x.Status != CampaignStatus.Pending)
                .WithInclude(x => x.Advertiser)
                .Build());

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByStatus(CampaignStatus? status, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && (!status.HasValue || x.Status == status.Value))
                .WithInclude(x => x.Advertiser)
                .Build());

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByStatuses(List<CampaignStatus>? statuses, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && (statuses == null || statuses.Count == 0 || statuses.Contains(x.Status)))
                .WithInclude(x => x.Advertiser)
                .Build());

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CampaignParticipationResponseDTO>> GetPublisherPaticipationByStatusForAdvertiser(int advertiserId, CampaignParticipationStatus? campaignParticipationStatus, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<CampaignParticipation>();
            var campaignParticipations = repo.Get(new QueryBuilder<CampaignParticipation>()
                .WithPredicate(x => x.Campaign.Advertiser.UserId == advertiserId
                && (!campaignParticipationStatus.HasValue || x.Status == campaignParticipationStatus))
                .WithInclude(x => x.Campaign)
                .Build());

            var pagedCampaigns = await PaginatedList<CampaignParticipation>.CreateAsync(campaignParticipations, pageIndex, pageSize);
            var response = _mapper.Map<List<CampaignParticipationResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignParticipationResponseDTO>(response, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByAdvertiserId(int advertiserId, CampaignStatus? status, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                 .WithPredicate(x => !x.IsDeleted
                            && x.AdvertiserId == advertiserId
                            && (!status.HasValue || x.Status == status))
                .WithInclude(x => x.Advertiser)
                .Build());

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }


        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsJoinedByPublisher(int publisherId, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<CampaignParticipation>();
            var campaignsQuery = repo.Get(new QueryBuilder<CampaignParticipation>()
                .WithPredicate(x => x.PublisherId == publisherId && !x.Campaign.IsDeleted)
                .WithInclude(x => x.Campaign.Advertiser)
                .Build())
                .Select(x => x.Campaign); // Lấy danh sách chiến dịch từ CampaignParticipations

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaignsQuery, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);

            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }


        public async Task<PaginatedList<AdvertiserResponseDTO>> GetAdvertisersByPublisher(int publisherId, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<CampaignParticipation>();
            var advertisersQuery = repo.Get(new QueryBuilder<CampaignParticipation>()
                .WithPredicate(x => x.PublisherId == publisherId && x.Campaign.Advertiser != null)
                .WithInclude(x => x.Campaign.Advertiser)
                .Build())
                .Select(x => x.Campaign.Advertiser)
                .Distinct();

            var pagedAdvertisers = await PaginatedList<Advertiser>.CreateAsync(advertisersQuery, pageIndex, pageSize);
            var result = _mapper.Map<List<AdvertiserResponseDTO>>(pagedAdvertisers);

            return new PaginatedList<AdvertiserResponseDTO>(result, pagedAdvertisers.TotalItems, pageIndex, pageSize);
        }
        public async Task<PaginatedList<CampaignResponseForPublisherDTO>> GetAllCampaignForPublisher(int publisherId, int pageIndex, int pageSize)
        {
            var campaignRepo = _unitOfWork.GetRepo<Campaign>();

            var campaignsQuery = campaignRepo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && x.Status == CampaignStatus.Activing)
                .WithInclude(x => x.Advertiser)
                .WithInclude(x => x.CampaignParticipations.Where(p => p.PublisherId == publisherId)) 
                .Build());

            
            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaignsQuery, pageIndex, pageSize);

     
            var mappedCampaigns = _mapper.Map<List<CampaignResponseForPublisherDTO>>(pagedCampaigns);

          
            var participationDict = pagedCampaigns
                .SelectMany(c => c.CampaignParticipations) 
                .ToDictionary(p => p.CampaignId, p => p.Status);

          
            foreach (var campaignDto in mappedCampaigns)
            {
                if (participationDict.TryGetValue(campaignDto.Id, out var status))
                {
                    campaignDto.PublisherStatus = status;
                }
            }

        
            return new PaginatedList<CampaignResponseForPublisherDTO>(mappedCampaigns, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<CampaignResponseDTO> GetCampaignById(int id)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(x => x.Id == id && !x.IsDeleted)
                .WithInclude(x => x.Advertiser)
                .Build());

            if (campaign == null)
            {
                return null;
            }

            return _mapper.Map<CampaignResponseDTO>(campaign);
        }
        public async Task<BaseResponse> ValidateCampaignForTraffic(int campaignId)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(x => x.Id == campaignId && !x.IsDeleted)
                .Build());

            if (campaign == null)
            {
                return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không tồn tại." };
            }

            if (campaign.Status != CampaignStatus.Activing)
            {
                return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không ở trạng thái hoạt động." };
            }

            DateOnly currentTime = DateOnly.FromDateTime(DateTime.UtcNow);
            if (currentTime < campaign.StartDate || currentTime > campaign.EndDate)
            {
                return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không trong thời gian hoạt động." };
            }

            if (campaign.Advertiser != null && campaign.Advertiser.ApplicationUser.IsDeleted)
            {
                return new BaseResponse { IsSuccess = false, Message = "Nhà quảng cáo của chiến dịch này đã bị khóa." };
            }

            return new BaseResponse { IsSuccess = true, Message = "Chiến dịch hợp lệ để chạy traffic." };
        }
        public async Task<BaseResponse> RegisterForCampaign(CampaignParticipationCreateDTO dto)
        {
            try
            {
                
                var campaignRepo = _unitOfWork.GetRepo<Campaign>();
                var campaign = await campaignRepo.GetSingleAsync(new QueryBuilder<Campaign>()
                    .WithPredicate(x => x.Id == dto.CampaignId && !x.IsDeleted && x.Status == CampaignStatus.Activing)
                    .Build());

                if (campaign == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không tồn tại hoặc không ở trạng thái hoạt động." };
                }

             
                var participationRepo = _unitOfWork.GetRepo<CampaignParticipation>();
                var existingParticipation = await participationRepo.GetSingleAsync(new QueryBuilder<CampaignParticipation>()
                    .WithPredicate(x => x.PublisherId == dto.PublisherId && x.CampaignId == dto.CampaignId)
                    .Build());

                if (existingParticipation != null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Bạn đã đăng ký chiến dịch này trước đó." };
                }

            
                var participation = new CampaignParticipation
                {
                    CampaignId = dto.CampaignId,
                    PublisherId = dto.PublisherId,
                    ShortLink = dto.ShortLink,
                    Status = CampaignParticipationStatus.Pending,
                    CreateAt = DateTime.UtcNow
                };

                await participationRepo.CreateAsync(participation);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse { IsSuccess = true, Message = "Đăng ký chiến dịch thành công. Vui lòng chờ xử lý." };
            }
            catch (Exception ex)
            {
                return new BaseResponse { IsSuccess = false, Message = "Đã xảy ra lỗi khi đăng ký chiến dịch." };
            }
        }

        public async Task UpdateCampaignBudgetAsync(int campaignId, int revenue)
        {
            var campaignRepo = _unitOfWork.GetRepo<Campaign>();
            var campaign = await campaignRepo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(c => c.Id == campaignId)
                .Build());

            if (campaign != null)
            {
                campaign.RemainingBudget -= revenue;
                await campaignRepo.UpdateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task CheckAndStopExpiredCampaigns()
        {
            var campaignRepo = _unitOfWork.GetRepo<Campaign>();

            var campaigns = await campaignRepo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(c => c.Status == CampaignStatus.Activing && !c.IsDeleted)
                .WithOrderBy(query => query.OrderBy(c => c.RemainingBudget))
                .Build())
                .Select(c => new { c.Id, c.RemainingBudget })
                .Take(100)
                .ToListAsync();

            foreach (var campaign in campaigns)
            {
                await CheckAndStopCampaignIfBudgetExceededAsync(campaign.Id);
            }
        }
        public async Task CheckAndStopCampaignIfBudgetExceededAsync(int campaignId)
        {
            var campaignRepo = _unitOfWork.GetRepo<Campaign>();
            var campaign = await campaignRepo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(c => c.Id == campaignId)
                .Build());

            if (campaign.RemainingBudget <= 0 || campaign.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                campaign.Status = CampaignStatus.Completed;
                await campaignRepo.UpdateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}
