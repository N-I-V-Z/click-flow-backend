using AutoMapper;
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

namespace ClickFlow.BLL.Services.Implements
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdvertiserService _advertiserService;
        private readonly ITrafficService _trafficService;



        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IAdvertiserService advertiserService, ITrafficService trafficService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _advertiserService = advertiserService;
            _trafficService = trafficService;
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

                DateTime startDate = DateTime.ParseExact(dto.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(dto.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime currentTime = DateTime.UtcNow.Date;

                if (startDate >= endDate)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Ngày bắt đầu không được lớn hơn ngày kêt thúc" };
                }
                if (startDate >= currentTime)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại." };
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

        public async Task<PaginatedList<CampaignParticipationResponseDTO>> GetPublisherPaticipationByStatusForAdvertiser(
            int advertiserId,
            CampaignParticipationStatus? campaignParticipationStatus,
            int pageIndex,
            int pageSize)
        {
            var repo = _unitOfWork.GetRepo<CampaignParticipation>();

            var campaignParticipations = repo.Get(new QueryBuilder<CampaignParticipation>()
                .WithPredicate(x => x.Campaign.Advertiser.UserId == advertiserId
                    && (!campaignParticipationStatus.HasValue || x.Status == campaignParticipationStatus))
                .WithInclude(x => x.Campaign)
                .WithInclude(x => x.Publisher)
                .WithInclude(x => x.Publisher.ApplicationUser)
                .Build());


            var pagedCampaigns = await PaginatedList<CampaignParticipation>.CreateAsync(campaignParticipations, pageIndex, pageSize);

            var queryOptions = new QueryBuilder<CampaignParticipation>()
                .WithInclude(x => x.Publisher)
                .Build();

            var allCampaignParticipations = repo.Get(queryOptions).ToList();

            // Tính TotalCampaigns cho từng publisher
            var publisherCampaignCount = allCampaignParticipations
                .GroupBy(x => x.PublisherId)
                .Select(g => new { PublisherId = g.Key, TotalCampaigns = g.Count() })
                .ToDictionary(x => x.PublisherId, x => x.TotalCampaigns);


            var response = _mapper.Map<List<CampaignParticipationResponseDTO>>(pagedCampaigns);

            // Lấy danh sách các publisherId duy nhất từ response
            var publisherIds = response.Select(x => x.PublisherId).Distinct().ToList();

            // Tính DailyTraffic cho tất cả publisher
            var avgTrafficDict = new Dictionary<int, int>();
            foreach (var publisherId in publisherIds)
            {
                avgTrafficDict[publisherId] = await _trafficService.AverageTrafficInCampaign(publisherId);
            }

            // Gán TotalCampaigns và DailyTraffic vào từng CampaignParticipationResponseDTO
            foreach (var item in response)
            {
                item.TotalCampaigns = publisherCampaignCount.ContainsKey(item.PublisherId)
                    ? publisherCampaignCount[item.PublisherId]
                    : 0;

                item.DailyTraffic = avgTrafficDict.GetValueOrDefault(item.PublisherId, 0);
            }


            return new PaginatedList<CampaignParticipationResponseDTO>(response, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }


        public async Task<BaseResponse> UpdateCampaignParticipationStatus(
            int publisherId,
            int advertiserId,
            int campaignParticipationId,
            CampaignParticipationStatus newStatus)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var advertiserRepo = _unitOfWork.GetRepo<Advertiser>();
                var advertiser = await advertiserRepo.GetSingleAsync(new QueryBuilder<Advertiser>()
                    .WithPredicate(x => x.Id == advertiserId)
                    .Build());

                if (advertiser == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy Advertiser." };
                }

                var publisherRepo = _unitOfWork.GetRepo<Publisher>();
                var publisher = await publisherRepo.GetSingleAsync(new QueryBuilder<Publisher>()
                    .WithPredicate(x => x.Id == publisherId)
                    .Build());

                if (publisher == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy Publisher." };
                }

                var participationRepo = _unitOfWork.GetRepo<CampaignParticipation>();
                var participation = await participationRepo.GetSingleAsync(new QueryBuilder<CampaignParticipation>()
                    .WithPredicate(x => x.Id == campaignParticipationId
                        && x.PublisherId == publisherId
                        && x.Campaign.AdvertiserId == advertiserId)
                    .WithInclude(x => x.Campaign)
                    .Build());

                if (participation == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy thông tin tham gia chiến dịch." };
                }

                if (participation.Status == newStatus)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Trạng thái đã được cập nhật trước đó." };
                }

                participation.Status = newStatus;

                await participationRepo.UpdateAsync(participation);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new BaseResponse { IsSuccess = true, Message = "Cập nhật trạng thái thành công." };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                return new BaseResponse { IsSuccess = false, Message = $"Đã xảy ra lỗi: {ex.Message}" };
            }
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
                .Select(x => x.Campaign);

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaignsQuery, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);

            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
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
        public async Task<PaginatedList<CampaignResponseDTO>> GetSimilarCampaignsByTypeCampaign(int campaignId, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();


            var currentCampaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(x => x.Id == campaignId && !x.IsDeleted)
                .Build());

            if (currentCampaign == null)
            {
                throw new Exception("Chiến dịch không tồn tại hoặc đã bị xóa.");
            }


            var campaignsQuery = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && x.TypeCampaign == currentCampaign.TypeCampaign && x.Status == CampaignStatus.Activing && x.AverageStarRate.HasValue && x.Id != campaignId)
                .WithInclude(x => x.Advertiser)
                .WithOrderBy(query => query.OrderByDescending(c => c.AverageStarRate))
                .Build());


            var topCampaigns = await campaignsQuery
                .Take(10)
                .ToListAsync();


            var result = _mapper.Map<List<CampaignResponseDTO>>(topCampaigns);


            return new PaginatedList<CampaignResponseDTO>(result, topCampaigns.Count, pageIndex, pageSize);
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

        public async Task<CampaignResponseForPublisherDTO> GetCampaignByIdForPublisher(int campaignId, int publisherId)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();

            var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
                .WithPredicate(x => x.Id == campaignId && !x.IsDeleted && x.Status == CampaignStatus.Activing)
                .WithInclude(x => x.Advertiser)
                .WithInclude(x => x.CampaignParticipations.Where(p => p.PublisherId == publisherId))
                .Build());

            if (campaign == null)
            {
                return null;
            }

            var response = _mapper.Map<CampaignResponseForPublisherDTO>(campaign);

            var participation = campaign.CampaignParticipations.FirstOrDefault();
            if (participation != null)
            {
                response.PublisherStatus = participation.Status;
            }

            return response;
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
        public async Task<BaseResponse> RegisterForCampaign(CampaignParticipationCreateDTO dto, int userId)
        {
            try
            {
                var publisherRepo = _unitOfWork.GetRepo<Publisher>();
                var publisher = await publisherRepo.GetSingleAsync(new QueryBuilder<Publisher>()
                    .WithPredicate(x => x.UserId == userId)
                    .Build());

                if (publisher == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Người dùng không phải Publisher hoặc chưa được đăng ký." };
                }

                int publisherId = publisher.Id;


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
                    .WithPredicate(x => x.PublisherId == publisherId && x.CampaignId == dto.CampaignId)
                    .Build());

                if (existingParticipation != null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Bạn đã đăng ký chiến dịch này trước đó." };
                }


                var participation = new CampaignParticipation
                {
                    CampaignId = dto.CampaignId,
                    PublisherId = publisherId,
                    ShortLink = "ShortLink",
                    Status = CampaignParticipationStatus.Pending,
                    CreateAt = DateTime.UtcNow
                };

                await participationRepo.CreateAsync(participation);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse { IsSuccess = true, Message = "Đăng ký chiến dịch thành công. Vui lòng chờ xử lý." };
            }
            catch (Exception)
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
        public async Task UpdateCampaignActiveStatus()
        {
            var campaignRepo = _unitOfWork.GetRepo<Campaign>();

            var campaigns = await campaignRepo.Get(new QueryBuilder<Campaign>()
              .WithPredicate(c => c.Status == CampaignStatus.Approved && !c.IsDeleted).Build()).ToListAsync();

            DateOnly currentTime = DateOnly.FromDateTime(DateTime.UtcNow);
            foreach (var c in campaigns)
            {
                if (c.StartDate == currentTime) c.Status = CampaignStatus.Activing;
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
        public async Task<int> GetCampaignCountByStatuses(List<CampaignStatus>? statuses)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && (statuses == null || statuses.Count == 0 || statuses.Contains(x.Status)))
                .Build());

            return await campaigns.CountAsync();
        }
        public async Task<int> GetPublisherParticipationCountByStatusForAdvertiser(
    int advertiserId,
    CampaignParticipationStatus? campaignParticipationStatus)
        {
            var repo = _unitOfWork.GetRepo<CampaignParticipation>();

            var campaignParticipations = repo.Get(new QueryBuilder<CampaignParticipation>()
                .WithPredicate(x => x.Campaign.Advertiser.UserId == advertiserId
                    && (!campaignParticipationStatus.HasValue || x.Status == campaignParticipationStatus.Value))
                .Build());

            return await campaignParticipations.CountAsync();
        }

        public async Task<int> GetCampaignCountByAdvertiserId(int advertiserId, CampaignStatus? status)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted
                           && x.AdvertiserId == advertiserId
                           && (!status.HasValue || x.Status == status))
                .Build());

            return await campaigns.CountAsync();
        }
    }
}
