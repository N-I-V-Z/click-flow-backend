using AutoMapper;
using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using System.Globalization;

namespace ClickFlow.BLL.Services.Implements
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> CreateCampaign(CampaignCreateDTO dto, string userId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Campaign>();

                var campaign = _mapper.Map<Campaign>(dto);

                campaign.Status = CampaignStatus.Pending;

                await repo.CreateAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Chiến dịch đã được tạo thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
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

        public async Task<BaseResponse> DeleteCampaign(int id, string userId)
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

        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsByAdvertiserId(int advertiserId, CampaignStatus status,int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Campaign>();
            var campaigns = repo.Get(new QueryBuilder<Campaign>()
                .WithPredicate(x => !x.IsDeleted && x.AdvertiserId == advertiserId && x.Status == status)
                .WithInclude(x => x.Advertiser)
                .Build());

            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns, pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CampaignResponseDTO>> GetCampaignsJoinedByPublisher(int publisherId, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Traffic>();
            var traffics = repo.Get(new QueryBuilder<Traffic>()
                .WithPredicate(x => x.PublisherId == publisherId)
                .WithInclude(x => x.Campaign)
                .Build());

            var campaigns = traffics.Select(x => x.Campaign).Where(x => !x.IsDeleted).ToList();
            var pagedCampaigns = await PaginatedList<Campaign>.CreateAsync(campaigns.AsQueryable(), pageIndex, pageSize);
            var result = _mapper.Map<List<CampaignResponseDTO>>(pagedCampaigns);
            return new PaginatedList<CampaignResponseDTO>(result, pagedCampaigns.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<AdvertiserResponseDTO>> GetAdvertisersByPublisher(int publisherId, int pageIndex, int pageSize)
        {
            var trafficRepo = _unitOfWork.GetRepo<Traffic>();
            var traffics = trafficRepo.Get(new QueryBuilder<Traffic>().WithPredicate(x => x.PublisherId == publisherId).WithInclude(x => x.Campaign.Advertiser).Build());

            var advertisers = traffics.Select(x => x.Campaign.Advertiser).Distinct().ToList();

            var pagedAdvertisers = await PaginatedList<Advertiser>.CreateAsync(advertisers.AsQueryable(), pageIndex, pageSize);

            var result = _mapper.Map<List<AdvertiserResponseDTO>>(pagedAdvertisers);
            return new PaginatedList<AdvertiserResponseDTO>(result, pagedAdvertisers.TotalItems, pageIndex, pageSize);

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
    }
}
