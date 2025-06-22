using AutoMapper;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.TrafficDTOs;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

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

		public async Task<BaseResponse> ValidateTraffic(TrafficCreateDTO dto)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<bool> IsValidTraffic(TrafficCreateDTO dto, string IpAddress)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
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
				newTraffic.IsValid = await IsValidTraffic(dto, remoteIp);

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
					await _unitOfWork.SaveChangesAsync();
				}

				// Nếu là CPS hoặc CPA → chỉ ghi nhận traffic, đợi postback từ advertiser
				// Postback sẽ dựa vào ClickId đã được sinh ở trên để tạo Conversion sau

				var saved = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				return saved ? new TrafficClickResponseDTO
				{
					ClickId = newTraffic.ClickId,
					URL = campaign.OriginURL
				} : null;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}



		public async Task<PaginatedList<TrafficResponseDTO>> GetAllAsync(PagingRequestDTO dto)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<TrafficResponseDTO> GetByIdAsync(int id)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByPublisherIdAsync(int id, TrafficForPublisherDTO dto)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByAdvertiserIdAsync(int id, PagingRequestDTO dto)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByCampaignIdAsync(int id, PagingRequestDTO dto)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task TransferTrafficToClosedTraffic()
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
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<int> AverageTrafficInCampaign(int publisherId)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
		public async Task<int> CountAllTrafficByCampaign(int campaignId)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<int> CountTrafficForPublisher(int campaignId, int publisherId)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<int> CountTrafficOfAllActiveCampaignForPublisher(int publisherId)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
