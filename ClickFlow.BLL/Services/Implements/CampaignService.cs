using AutoMapper;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.CampaignParticipationDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.Helpers.Fillters;
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

		private readonly double COMMISION = 0.9;

		public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IAdvertiserService advertiserService, ITrafficService trafficService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_advertiserService = advertiserService;
			_trafficService = trafficService;
		}

		protected virtual QueryBuilder<Campaign> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<Campaign>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<Campaign>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<BaseResponse> CreateCampaign(CampaignCreateDTO dto, int userId)
		{
			try
			{

				await _unitOfWork.BeginTransactionAsync();


				var advertiserRepo = _unitOfWork.GetRepo<Advertiser>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();
				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var userRepo = _unitOfWork.GetRepo<ApplicationUser>();


				var advertiser = await _advertiserService.GetAdvertiserByUserIdAsync(userId);

				if (advertiser == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy Advertiser." };
				}

				var advertiserWallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>().WithPredicate(w => w.UserId == userId).Build());
				if (advertiserWallet == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy ví của advertiser." };
				}
				var budget = (int)dto.Budget;
				if (advertiserWallet.Balance < budget)
				{
					return new BaseResponse { IsSuccess = false, Message = "Số dư không đủ để tạo chiến dịch." };
				}

				// Trừ tiền advertiser và cộng tiền cho admin
				var adminUser = await userRepo.GetSingleAsync(new QueryBuilder<ApplicationUser>().WithPredicate(u => u.Role == Role.Admin).Build());
				if (adminUser == null)
				{
					await _unitOfWork.RollBackAsync();
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tài khoản admin." };
				}
				var adminWallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>().WithPredicate(w => w.UserId == adminUser.Id).Build());
				if (adminWallet == null)
				{
					await _unitOfWork.RollBackAsync();
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy ví của admin." };
				}

				var commission = (int)(budget * 0.05);

				await ProcessCampaignPaymentAsync(advertiserWallet, adminWallet, budget, commission);


				DateTime startDate = DateTime.ParseExact(dto.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				DateTime endDate = DateTime.ParseExact(dto.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				DateTime currentTime = DateTime.UtcNow.Date;

				if (startDate >= endDate)
				{
					return new BaseResponse { IsSuccess = false, Message = "Ngày bắt đầu không được lớn hơn ngày kêt thúc" };
				}
				if (startDate < currentTime)
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

		private async Task ProcessCampaignPaymentAsync(
			Wallet advertiserWallet,
			Wallet adminWallet,
			int budget,
			int commission)
		{
			var walletRepo = _unitOfWork.GetRepo<Wallet>();
			var transactionRepo = _unitOfWork.GetRepo<Transaction>();

			advertiserWallet.Balance -= budget;
			adminWallet.Balance += commission;

			await walletRepo.UpdateAsync(advertiserWallet);
			await walletRepo.UpdateAsync(adminWallet);

			// Create advertiser transaction
			await transactionRepo.CreateAsync(new Transaction
			{
				WalletId = advertiserWallet.Id,
				Amount = budget,
				Balance = advertiserWallet.Balance,
				PaymentDate = DateTime.UtcNow,
				TransactionType = TransactionType.Pay,
				Status = true
			});

			// Create admin transaction
			await transactionRepo.CreateAsync(new Transaction
			{
				WalletId = adminWallet.Id,
				Amount = commission,
				Balance = adminWallet.Balance,
				PaymentDate = DateTime.UtcNow,
				TransactionType = TransactionType.Received,
				Status = true
			});
		}

		private async Task ProcessCampaignRefundAsync(
		Wallet advertiserWallet,
		Wallet adminWallet,
		int budget,
		int commission)
		{
			var walletRepo = _unitOfWork.GetRepo<Wallet>();
			var transactionRepo = _unitOfWork.GetRepo<Transaction>();

			advertiserWallet.Balance += budget;
			adminWallet.Balance -= commission;

			await walletRepo.UpdateAsync(advertiserWallet);
			await walletRepo.UpdateAsync(adminWallet);

			// Tạo transaction hoàn tiền cho advertiser
			var advertiserTransaction = new Transaction
			{
				WalletId = advertiserWallet.Id,
				Amount = budget,
				Balance = advertiserWallet.Balance,
				PaymentDate = DateTime.UtcNow,
				TransactionType = TransactionType.Deposit,
				Status = true
			};
			await transactionRepo.CreateAsync(advertiserTransaction);

			// Tạo transaction trừ tiền cho admin
			var adminTransaction = new Transaction
			{
				WalletId = adminWallet.Id,
				Amount = commission,
				Balance = adminWallet.Balance,
				PaymentDate = DateTime.UtcNow,
				TransactionType = TransactionType.Withdraw,
				Status = true
			};
			await transactionRepo.CreateAsync(adminTransaction);
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
					.WithInclude(x => x.Advertiser)
					.WithTracking(true)
					.Build());

				if (campaign == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Chiến dịch không tồn tại." };
				}

				if (!dto.IsStatusValid())
				{
					return new BaseResponse { IsSuccess = false, Message = "Trạng thái không hợp lệ." };
				}

				if (dto.Status.Value == CampaignStatus.Canceled && campaign.Status == CampaignStatus.Pending)
				{
					var walletRepo = _unitOfWork.GetRepo<Wallet>();
					var transactionRepo = _unitOfWork.GetRepo<Transaction>();
					var userRepo = _unitOfWork.GetRepo<ApplicationUser>();

					var advertiserWallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>().WithPredicate(w => w.UserId == campaign.Advertiser.UserId).Build());
					if (advertiserWallet == null)
					{
						await _unitOfWork.RollBackAsync();
						return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy ví của advertiser." };
					}

					var adminUser = await userRepo.GetSingleAsync(new QueryBuilder<ApplicationUser>().WithPredicate(u => u.Role == Role.Admin).Build());
					if (adminUser == null)
					{
						await _unitOfWork.RollBackAsync();
						return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tài khoản admin." };
					}
					var adminWallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>().WithPredicate(w => w.UserId == adminUser.Id).Build());
					if (adminWallet == null)
					{
						await _unitOfWork.RollBackAsync();
						return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy ví của admin." };
					}

					var budget = (int)campaign.Budget;
					var commission = (int)(budget * 0.05);

					await ProcessCampaignRefundAsync(advertiserWallet, adminWallet, budget, commission);
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

		public async Task<CampaignResponseDTO> GetCampaignByTd(int Id)
		{
			var repo = _unitOfWork.GetRepo<Campaign>();
			var campaign = await repo.GetSingleAsync(new QueryBuilder<Campaign>()
				.WithPredicate(x => x.Id == Id && !x.IsDeleted)
				.WithInclude(x => x.Advertiser)
				.Build());
			if (campaign == null)
			{
				return null;
			}
			return _mapper.Map<CampaignResponseDTO>(campaign);
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
				avgTrafficDict[publisherId] = await _trafficService.AverageTrafficInCampaignAsync(publisherId);
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

		public async Task<PaginatedList<CampaignResponseForPublisherDTO>> GetAllCampaignForPublisher(int publisherId, int pageIndex, int pageSize, Industry? industry = null, TypePay? typePay = null)
		{
			var campaignRepo = _unitOfWork.GetRepo<Campaign>();

			var campaignsQuery = campaignRepo.Get(new QueryBuilder<Campaign>()
				.WithPredicate(x => !x.IsDeleted &&
								 x.Status == CampaignStatus.Activing &&
								 (!industry.HasValue || x.TypeCampaign == industry.Value) &&
								 (!typePay.HasValue || x.TypePay == typePay.Value))
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

		private async Task<double> GetTotalRevenueOfValidConversions(int campaignId)
		{
			var participationRepo = _unitOfWork.GetRepo<CampaignParticipation>();
			var trafficRepo = _unitOfWork.GetRepo<Traffic>();
			var conversionRepo = _unitOfWork.GetRepo<Conversion>();

			var participations = await participationRepo.Get(new QueryBuilder<CampaignParticipation>()
				.WithPredicate(x => x.CampaignId == campaignId)
				.Build()).ToListAsync();

			var participationIds = participations.Select(p => p.Id).ToList();

			var traffics = await trafficRepo.Get(new QueryBuilder<Traffic>()
				.WithPredicate(x => x.IsValid && x.CampaignParticipationId != null && participationIds.Contains(x.CampaignParticipationId.Value))
				.Build()).ToListAsync();

			var clickIds = traffics.Select(t => t.ClickId).ToList();

			var conversions = await conversionRepo.Get(new QueryBuilder<Conversion>()
				.WithPredicate(x => clickIds.Contains(x.ClickId) && x.Revenue != null)
				.Build()).ToListAsync();

			return conversions.Sum(c => c.Revenue ?? 0);
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
			var availableBudget = campaign.Budget * COMMISION;

			var totalRevenue = await GetTotalRevenueOfValidConversions(campaignId);

			// Lấy giá trị hoa hồng cho 1 lượt click
			var nextClickRevenue = campaign.Commission ?? (campaign.Percents / 100 * campaign.Budget) ?? 0;

			if (totalRevenue + nextClickRevenue >= availableBudget)
			{
				return new BaseResponse { IsSuccess = false, Message = $"Tổng doanh thu hiện tại cộng thêm 1 lượt click nữa sẽ vượt quá ngân sách khả dụng ({availableBudget:N0})." };
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

		public async Task<PaginatedList<CampaignParticipationResponseDTO>> GetPublishersInCampaign(int campaignId, int pageIndex, int pageSize)
		{
			var repo = _unitOfWork.GetRepo<CampaignParticipation>();

			var participations = repo.Get(new QueryBuilder<CampaignParticipation>()
				.WithPredicate(x => x.CampaignId == campaignId)
				.WithInclude(x => x.Publisher)
				.WithInclude(x => x.Publisher.ApplicationUser)
				.Build());

			var pagedParticipations = await PaginatedList<CampaignParticipation>.CreateAsync(participations, pageIndex, pageSize);

			// Map từng item thay vì cả collection
			var result = new List<CampaignParticipationResponseDTO>();
			foreach (var item in pagedParticipations)
			{
				var dto = _mapper.Map<CampaignParticipationResponseDTO>(item);
				result.Add(dto);
			}

			// Get all campaign participations for calculating TotalCampaigns
			var queryOptions = new QueryBuilder<CampaignParticipation>()
				.WithInclude(x => x.Publisher)
				.Build();

			var allCampaignParticipations = repo.Get(queryOptions).ToList();

			// Calculate TotalCampaigns for each publisher
			var publisherCampaignCount = allCampaignParticipations
				.GroupBy(x => x.PublisherId)
				.Select(g => new { PublisherId = g.Key, TotalCampaigns = g.Count() })
				.ToDictionary(x => x.PublisherId, x => x.TotalCampaigns);

			// Get unique publisher IDs
			var publisherIds = result.Select(x => x.PublisherId).Distinct().ToList();

			// Calculate DailyTraffic for all publishers
			var avgTrafficDict = new Dictionary<int, int>();
			foreach (var publisherId in publisherIds)
			{
				avgTrafficDict[publisherId] = await _trafficService.AverageTrafficInCampaignAsync(publisherId);
			}

			// Assign TotalCampaigns and DailyTraffic to each DTO
			foreach (var item in result)
			{
				item.TotalCampaigns = publisherCampaignCount.ContainsKey(item.PublisherId)
					? publisherCampaignCount[item.PublisherId]
					: 0;

				item.DailyTraffic = avgTrafficDict.GetValueOrDefault(item.PublisherId, 0);
			}

			return new PaginatedList<CampaignParticipationResponseDTO>(result, pagedParticipations.TotalItems, pageIndex, pageSize);
		}

		public async Task<CampaignCountGroupByStatusDTO> CountCampaignGroupByStatusAsync(int userId)
		{
			var repo = _unitOfWork.GetRepo<Campaign>();

			var statusCounts = await repo.Get(CreateQueryBuilder()
				.WithPredicate(c => !c.IsDeleted && c.AdvertiserId == userId)
				.Build())
				.GroupBy(c => c.Status)
				.Select(g => new { Status = g.Key, Count = g.Count() })
				.ToListAsync();

			var result = new CampaignCountGroupByStatusDTO();

			foreach (var item in statusCounts)
			{
				switch (item.Status)
				{
					case CampaignStatus.Pending:
						result.PendingCount = item.Count;
						break;
					case CampaignStatus.Approved:
						result.ApprovedCount = item.Count;
						break;
					case CampaignStatus.Activing:
						result.ActivingCount = item.Count;
						break;
					case CampaignStatus.Paused:
						result.PausedCount = item.Count;
						break;
					case CampaignStatus.Canceled:
						result.CanceledCount = item.Count;
						break;
					case CampaignStatus.Completed:
						result.CompletedCount = item.Count;
						break;
				}
			}

			return result;
		}
	}
}
