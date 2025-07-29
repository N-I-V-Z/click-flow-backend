using AutoMapper;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ClickFlow.BLL.Services.Implements
{
	public class TrafficService : BaseServices<Traffic, TrafficResponseDTO>, ITrafficService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly double COMMISSION = 0.05;
		public TrafficService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<BaseResponse> ValidateTrafficAsync(TrafficCreateDTO dto)
		{
			var campaignParticipationRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var cp = await campaignParticipationRepo.GetSingleAsync(new QueryBuilder<CampaignParticipation>()
				.WithPredicate(x =>
					x.CampaignId == dto.CampaignId &&
					x.PublisherId == dto.PublisherId &&
					x.Status == CampaignParticipationStatus.Participated
				).Build());

			if (cp == null)
			{
				return new BaseResponse
				{
					IsSuccess = false,
					Message = "Publihser chưa tham gia chiến dịch này"
				};
			}

			return new BaseResponse
			{
				IsSuccess = true
			};
		}

		public async Task<bool> IsValidTrafficAsync(TrafficCreateDTO dto, string IpAddress)
		{
			var campaignParticipationRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var cp = await campaignParticipationRepo.GetSingleAsync(new QueryBuilder<CampaignParticipation>()
				.WithPredicate(x =>
					x.CampaignId == dto.CampaignId &&
					x.PublisherId == dto.PublisherId &&
					x.Status == CampaignParticipationStatus.Participated
				).Build());

			var today = DateTime.UtcNow.Date;

			var queryBuilder = CreateQueryBuilder();
			var checkIpQueryOptions = queryBuilder.WithPredicate(x =>
				x.CampaignParticipationId == cp.Id &&
				x.IpAddress.Equals(IpAddress) &&
				x.IsValid == true &&
				x.Timestamp.Date == today.Date
			);

			var trafficRepo = _unitOfWork.GetRepo<Traffic>();
			bool checkIpAddress = await trafficRepo.GetSingleAsync(checkIpQueryOptions.Build()) == null;

			if (!checkIpAddress)
			{
				return false;
			}

			return true;
		}
		public async Task<TrafficClickResponseDTO> CreateAsync(TrafficCreateDTO dto, string remoteIp)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var trafficRepo = _unitOfWork.GetRepo<Traffic>();
				var campaignRepo = _unitOfWork.GetRepo<Campaign>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();
				var cpRepo = _unitOfWork.GetRepo<CampaignParticipation>();
				var conversionRepo = _unitOfWork.GetRepo<Conversion>();

				// Lấy thông tin chiến dịch và người tham gia
				var campaign = await campaignRepo.GetSingleAsync(
					new QueryBuilder<Campaign>().WithPredicate(x => x.Id == dto.CampaignId).Build());

				var participation = await cpRepo.GetSingleAsync(
					new QueryBuilder<CampaignParticipation>()
						.WithPredicate(x => x.CampaignId == dto.CampaignId && x.PublisherId == dto.PublisherId)
						.Build());

				var wallet = await walletRepo.GetSingleAsync(
					new QueryBuilder<Wallet>().WithPredicate(x => x.UserId == dto.PublisherId).Build());

				// Ghi nhận lượt click
				var newTraffic = _mapper.Map<Traffic>(dto);
				newTraffic.ClickId = Guid.NewGuid().ToString();
				newTraffic.Timestamp = dto.Timestamp ?? DateTime.UtcNow;
				newTraffic.CampaignParticipationId = participation.Id;
				newTraffic.IpAddress = remoteIp;
				newTraffic.IsValid = await IsValidTrafficAsync(dto, remoteIp);

				await trafficRepo.CreateAsync(newTraffic);

				var commision = (campaign.Commission != null ? campaign.Commission.Value : campaign.Budget * campaign.Percents / 100) * (1 - COMMISSION);

				// Nếu là CPC → tạo conversion ngay lập tức
				if (campaign.TypePay == TypePay.CPC && newTraffic.IsValid)
				{
					var conversion = new Conversion
					{
						ClickId = newTraffic.ClickId,
						EventType = ConversionEventType.Conversion,
						Revenue = (int)commision,
						OrderId = null,
						Timestamp = DateTime.UtcNow
					};

					await conversionRepo.CreateAsync(conversion);
					await _unitOfWork.SaveChangesAsync();

					// Cộng hoa hồng cho publisher
					wallet.Balance += (int)commision;
					await walletRepo.UpdateAsync(wallet);
					await _unitOfWork.SaveChangesAsync();

					var userRepo = _unitOfWork.GetRepo<ApplicationUser>();
					var admin = await userRepo.GetSingleAsync(new QueryBuilder<ApplicationUser>().WithPredicate(x => x.Role == Role.Admin).Build());
					var adminWallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>().WithPredicate(x => x.UserId == admin.Id).Build());

					var adminCommision = (int)((campaign.Commission != null ? campaign.Commission.Value : campaign.Budget * campaign.Percents / 100) * COMMISSION);
					adminWallet.Balance += adminCommision;

					var adminTranssaction = new Transaction
					{
						Amount = adminCommision,
						Balance = adminWallet.Balance,
						PaymentDate = DateTime.UtcNow,
						Status = true,
						TransactionType = TransactionType.Received,
						WalletId = adminWallet.Id
					};
				}

				// Nếu là CPS hoặc CPA → chỉ ghi nhận traffic, đợi postback từ advertiser
				// Postback sẽ dựa vào ClickId đã được sinh ở trên để tạo Conversion sau

				var saved = await _unitOfWork.SaveAsync();

				if (saved == true)
				{
					await _unitOfWork.CommitTransactionAsync();
				}
				else
				{
					await _unitOfWork.RollBackAsync();
				}

				return saved ? new TrafficClickResponseDTO
				{
					ClickId = newTraffic.ClickId,
					URL = campaign.OriginURL
				} : null;
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollBackAsync();
				throw new Exception(ex.Message);
			}
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllAsync(PagingRequestDTO dto)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword).WithInclude(
				//x => x.CampaignParticipation, 
				x => x.CampaignParticipation.Campaign,
				x => x.CampaignParticipation.Publisher.ApplicationUser);

			queryBuilder.WithOrderBy(x => x.OrderByDescending(x => x.Timestamp));

			var loadedRecords = trafficRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<TrafficResponseDTO> GetByIdAsync(int id)
		{
			var queryBuilder = CreateQueryBuilder();
			var queryOptions = queryBuilder
				.WithPredicate(x => x.Id == id)
				.WithInclude(
					//x => x.CampaignParticipation,
					x => x.CampaignParticipation.Campaign,
					x => x.CampaignParticipation.Publisher.ApplicationUser);

			var trafficRepo = _unitOfWork.GetRepo<Traffic>();
			var response = await trafficRepo.GetSingleAsync(queryOptions.Build());
			if (response == null) return null;
			return _mapper.Map<TrafficResponseDTO>(response);
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByPublisherIdAsync(int id, TrafficForPublisherDTO dto)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword);
			var queryOptions = queryBuilder
				.WithInclude(
					//x => x.CampaignParticipation, 
					x => x.CampaignParticipation.Campaign,
					x => x.CampaignParticipation.Publisher.ApplicationUser)
				.WithPredicate(x => x.CampaignParticipation.Publisher.ApplicationUser.Id == id);

			if (dto.CampaignId != null)
			{
				queryBuilder.WithPredicate(x => x.CampaignParticipation.CampaignId == dto.CampaignId);
			}

			queryBuilder.WithOrderBy(x => x.OrderByDescending(x => x.Timestamp));

			var loadedRecords = trafficRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByAdvertiserIdAsync(int id, PagingRequestDTO dto)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword);
			var queryOptions = queryBuilder.WithInclude(
					//x => x.CampaignParticipation,
					x => x.CampaignParticipation.Campaign.Advertiser.ApplicationUser,
					x => x.CampaignParticipation.Publisher.ApplicationUser)
				.WithPredicate(x => x.CampaignParticipation.Campaign.Advertiser.ApplicationUser.Id == id);

			queryBuilder.WithOrderBy(x => x.OrderByDescending(x => x.Timestamp));

			var loadedRecords = trafficRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByCampaignIdAsync(int id, PagingRequestDTO dto)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword);
			var queryOptions = queryBuilder.WithInclude(
					//x => x.CampaignParticipation,
					x => x.CampaignParticipation.Campaign,
					x => x.CampaignParticipation.Publisher.ApplicationUser)
				.WithPredicate(x => x.CampaignParticipation.CampaignId == id)
				.WithOrderBy(x => x.OrderByDescending(x => x.Timestamp));

			var loadedRecords = trafficRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task TransferTrafficToClosedTrafficAsync()
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var trafficRepo = _unitOfWork.GetRepo<Traffic>();

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder
					.WithPredicate(x =>
						(x.CampaignParticipation.Campaign.EndDate <= DateOnly.FromDateTime(DateTime.UtcNow)) ||
						(x.CampaignParticipation.Campaign.Status == CampaignStatus.Canceled) ||
						(x.CampaignParticipation.Campaign.Status == CampaignStatus.Completed));

				var trafficsToClose = await trafficRepo.GetAllAsync(queryOptions.Build());
				foreach (var traffic in trafficsToClose)
				{
					traffic.IsClosed = true;
				}

				await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollBackAsync();
				throw new Exception(ex.Message);
			}
		}

		public async Task<int> AverageTrafficInCampaignAsync(int publisherId)
		{
			var cpRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();
			var campaignRepo = _unitOfWork.GetRepo<Campaign>();

			var campaignList = await cpRepo.GetAllAsync(new QueryBuilder<CampaignParticipation>()
				.WithPredicate(x => x.PublisherId == publisherId)
				.Build());

			if (!campaignList.Any()) return 0;

			var campaignIds = campaignList.Select(x => x.CampaignId).ToList();

			var campaigns = await campaignRepo.GetAllAsync(new QueryBuilder<Campaign>()
				.WithPredicate(x => campaignIds.Contains(x.Id) &&
								   (x.Status == CampaignStatus.Completed || x.Status == CampaignStatus.Canceled))
				.Build());

			if (!campaigns.Any()) return 0;

			var validCpIds = campaignList
				.Where(x => campaigns.Any(c => c.Id == x.CampaignId))
				.Select(x => x.Id)
				.ToList();

			if (!validCpIds.Any()) return 0;

			var trafficList = await trafficRepo.GetAllAsync(new QueryBuilder<Traffic>()
				.WithPredicate(x => x.IsClosed && validCpIds.Contains((int)x.CampaignParticipationId))
				.Build());

			if (!trafficList.Any()) return 0;

			var avg = trafficList
				.GroupBy(x => x.CampaignParticipationId)
				.Select(g => g.Count())
				.Average();

			return (int)avg;
		}
		public async Task<int> CountAllTrafficByCampaignAsync(int campaignId)
		{
			var campaignRepo = _unitOfWork.GetRepo<Campaign>();
			var cpRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var campaign = await campaignRepo.GetSingleAsync(
				new QueryBuilder<Campaign>()
					.WithPredicate(x => x.Id == campaignId)
					.Build());

			if (campaign == null) throw new Exception($"Campaign with ID {campaignId} not found.");

			var cps = await cpRepo.GetAllAsync(
				new QueryBuilder<CampaignParticipation>()
					.WithPredicate(x => x.CampaignId == campaignId)
					.Build());

			var cpIds = cps.Select(x => x.Id).ToList();
			if (!cpIds.Any()) return 0;

			var isClosed = campaign.Status == CampaignStatus.Completed || campaign.Status == CampaignStatus.Canceled;

			var traffics = await trafficRepo.GetAllAsync(
				new QueryBuilder<Traffic>()
					.WithPredicate(x => x.IsClosed == isClosed && cpIds.Contains((int)x.CampaignParticipationId))
					.Build());

			return traffics.Count();
		}

		public async Task<int> CountTrafficForPublisherAsync(int campaignId, int publisherId)
		{
			var campaignRepo = _unitOfWork.GetRepo<Campaign>();
			var cpRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var campaign = await campaignRepo.GetSingleAsync(
				new QueryBuilder<Campaign>()
					.WithPredicate(x => x.Id == campaignId)
					.Build());

			if (campaign == null) throw new Exception($"Campaign with ID {campaignId} not found.");

			var cps = await cpRepo.GetSingleAsync(
				new QueryBuilder<CampaignParticipation>()
					.WithPredicate(x => x.CampaignId == campaignId && x.PublisherId == publisherId)
					.Build());

			if (cps == null) return 0;

			var isClosed = campaign.Status == CampaignStatus.Completed || campaign.Status == CampaignStatus.Canceled;

			var traffics = await trafficRepo.GetAllAsync(
				CreateQueryBuilder()
					.WithPredicate(x => x.IsClosed == isClosed && cps.Id == x.CampaignParticipationId && x.IsValid)
					.Build());

			return traffics.Count();
		}

		public async Task<int> CountTrafficOfAllActiveCampaignForPublisherAsync(int publisherId)
		{
			var cpRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var cps = await cpRepo.GetSingleAsync(
				new QueryBuilder<CampaignParticipation>()
					.WithPredicate(x => x.PublisherId == publisherId)
					.Build());

			if (cps == null) return 0;


			var traffics = await trafficRepo.GetAllAsync(
				CreateQueryBuilder()
					.WithPredicate(x => x.IsClosed == false && cps.Id == x.CampaignParticipationId && x.IsValid)
					.Build());

			return traffics.Count();
		}

		public async Task<List<TrafficBrowserStatisticsDTO>> GetBrowserStatisticsAsync(int publisherId)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(t =>
					t.IsValid &&
					t.CampaignParticipation != null &&
					t.CampaignParticipation.PublisherId == publisherId &&
					t.CampaignParticipation.Campaign != null &&
					t.CampaignParticipation.Campaign.Status == CampaignStatus.Activing &&
					!t.CampaignParticipation.Campaign.IsDeleted
				);

			var query = trafficRepo.Get(queryBuilder.Build());

			var browserGroups = await query
				.GroupBy(t => t.Browser)
				.Select(g => new TrafficBrowserStatisticsDTO
				{
					Browser = g.Key,
					ClickCount = g.Count()
				})
				.ToListAsync();

			var totalClicks = browserGroups.Sum(x => x.ClickCount);

			var result = browserGroups
				.Select(x => new TrafficBrowserStatisticsDTO
				{
					Browser = x.Browser,
					ClickCount = x.ClickCount,
					ClickRate = totalClicks > 0
						? (int)Math.Round(100.0 * x.ClickCount / totalClicks)
						: 0
				})
				.OrderByDescending(x => x.ClickCount)
				.ToList();

			return result;
		}

		public async Task<List<TrafficDeviceStatisticsDTO>> GetDeviceStatisticsAsync(int publisherId)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(t =>
					t.IsValid &&
					t.CampaignParticipation != null &&
					t.CampaignParticipation.PublisherId == publisherId &&
					t.CampaignParticipation.Campaign != null &&
					t.CampaignParticipation.Campaign.Status == CampaignStatus.Activing &&
					!t.CampaignParticipation.Campaign.IsDeleted
				);

			var query = trafficRepo.Get(queryBuilder.Build());

			var deviceStats = await query
				.GroupBy(t => t.DeviceType)
				.Select(g => new TrafficDeviceStatisticsDTO
				{
					DeviceType = g.Key,
					Count = g.Count()
				})
				.OrderByDescending(x => x.Count)
				.ToListAsync();

			return deviceStats;
		}

		public async Task<List<TrafficBrowserStatisticsDTO>> GetBrowserStatisticsByCampaignAsync(int campaignId)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(t =>
					t.IsValid &&
					t.CampaignParticipation != null &&
					t.CampaignParticipation.CampaignId == campaignId
				);

			var query = trafficRepo.Get(queryBuilder.Build());

			var browserGroups = await query
				.GroupBy(t => t.Browser)
				.Select(g => new TrafficBrowserStatisticsDTO
				{
					Browser = g.Key,
					ClickCount = g.Count()
				})
				.ToListAsync();

			var totalClicks = browserGroups.Sum(x => x.ClickCount);

			var result = browserGroups
				.Select(x => new TrafficBrowserStatisticsDTO
				{
					Browser = x.Browser,
					ClickCount = x.ClickCount,
					ClickRate = totalClicks > 0
						? (int)Math.Round(100.0 * x.ClickCount / totalClicks)
						: 0
				})
				.OrderByDescending(x => x.ClickCount)
				.ToList();

			return result;
		}
		public async Task<List<TrafficDeviceStatisticsDTO>> GetDeviceStatisticsByCampaignAsync(int campaignId)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(t =>
					t.IsValid &&
					t.CampaignParticipation != null &&
					t.CampaignParticipation.CampaignId == campaignId
				);

			var query = trafficRepo.Get(queryBuilder.Build());

			var deviceStats = await query
				.GroupBy(t => t.DeviceType)
				.Select(g => new TrafficDeviceStatisticsDTO
				{
					DeviceType = g.Key,
					Count = g.Count()
				})
				.OrderByDescending(x => x.Count)
				.ToListAsync();

			return deviceStats;
		}

		public async Task<List<TrafficRevenueDTO>> GetRevenuesForPublisherAsync(int publisherId)
		{
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();

			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(t =>
					t.IsValid &&
					t.CampaignParticipation != null &&
					t.CampaignParticipation.PublisherId == publisherId &&
					t.CampaignParticipation.Campaign != null &&
					t.CampaignParticipation.Campaign.Status == CampaignStatus.Activing &&
					!t.CampaignParticipation.Campaign.IsDeleted &&
					t.Conversions != null && t.Conversions.Any(c => c.Revenue != null)
				);

			var query = trafficRepo.Get(queryBuilder.Build());

			var revenueGroups = await query
				.SelectMany(t => t.Conversions
					.Where(c => c.Revenue != null)
					.Select(c => new
					{
						CampaignName = t.CampaignParticipation.Campaign.Name,
						Revenue = c.Revenue ?? 0
					})
				)
				.GroupBy(x => x.CampaignName)
				.Select(g => new TrafficRevenueDTO
				{
					CampaignName = g.Key,
					Revenue = g.Sum(x => x.Revenue)
				})
				.OrderByDescending(x => x.Revenue)
				.ToListAsync();

			return revenueGroups;
		}
	}
}
