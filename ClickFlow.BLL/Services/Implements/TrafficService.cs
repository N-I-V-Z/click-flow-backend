﻿using AutoMapper;
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
using System.Linq;

namespace ClickFlow.BLL.Services.Implements
{
	public class TrafficService : ITrafficService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public TrafficService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		protected virtual QueryBuilder<Traffic> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<Traffic>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<Traffic>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetPagedData(IQueryable<Transaction> query, int pageIndex, int pageSize)
		{
			var paginatedEntities = await PaginatedList<Transaction>.CreateAsync(query, pageIndex, pageSize);
			var resultDto = _mapper.Map<List<TrafficResponseDTO>>(paginatedEntities);

			return new PaginatedList<TrafficResponseDTO>(resultDto, paginatedEntities.TotalItems, pageIndex, pageSize);
		}
		public async Task<BaseResponse> ValidateTraffic(TrafficCreateDTO dto)
		{
			try
			{
				var campaignParticipationRepo = _unitOfWork.GetRepo<CampaignParticipation>();
				var cp = await campaignParticipationRepo.GetSingleAsync(new QueryBuilder<CampaignParticipation>()
					.WithPredicate(x => x.CampaignId == dto.CampaignId && x.PublisherId == dto.PublisherId).Build());

				var queryBuilder = CreateQueryBuilder();
				var checkIpQueryOptions = queryBuilder.WithPredicate(x =>
					x.CampaignParticipationId == cp.Id &&
					x.IpAddress.Equals(dto.IpAddress) &&
					x.IsValid == true
				);

				var trafficRepo = _unitOfWork.GetRepo<Traffic>();
				bool checkIpAddress = await trafficRepo.GetSingleAsync(checkIpQueryOptions.Build()) == null;

				if (!checkIpAddress)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Địa chỉ Ip đã tham gia chiến dịch từ publisher này"
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
		public async Task<string> CreateAsync(TrafficCreateDTO dto)
		{
			try
			{

				var trafficRepo = _unitOfWork.GetRepo<Traffic>();
				var campaignRepo = _unitOfWork.GetRepo<Campaign>();
				var campagin = await campaignRepo.GetSingleAsync(new QueryBuilder<Campaign>().WithPredicate(x => x.Id == dto.CampaignId).Build());

				// traffic được tạo
				var newTraffic = _mapper.Map<Traffic>(dto);

				newTraffic.Timestamp = DateTime.UtcNow;
				if (dto.IsValid == true)
				{
					if (campagin.TypePay == TypePay.CPC)
					{
						newTraffic.Revenue = campagin.Commission;
					}
				}
				else
				{
					newTraffic.Revenue = 0;
				}

				await trafficRepo.CreateAsync(newTraffic);
				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return String.Empty;
				}


				return campagin.OriginURL;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllAsync(PagingRequestDTO dto)
		{
			try
			{
				var trafficRepo = _unitOfWork.GetRepo<Traffic>();

				var queryBuilder = CreateQueryBuilder().WithInclude(
					//x => x.CampaignParticipation, 
					x => x.CampaignParticipation.Campaign,
					x => x.CampaignParticipation.Publisher.ApplicationUser);
				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}
				var loadedRecords = trafficRepo.Get(queryBuilder.Build());

				var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
				var resultDTO = _mapper.Map<List<TrafficResponseDTO>>(pagedRecords);
				return new PaginatedList<TrafficResponseDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
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

		public async Task<PaginatedList<TrafficResponseDTO>> GetAllByPublisherIdAsync(int id, PagingRequestDTO dto)
		{
			try
			{
				var trafficRepo = _unitOfWork.GetRepo<Traffic>();

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder
					.WithInclude(
						//x => x.CampaignParticipation, 
						x => x.CampaignParticipation.Campaign,
						x => x.CampaignParticipation.Publisher.ApplicationUser)
					.WithPredicate(x => x.CampaignParticipation.Publisher.ApplicationUser.Id == id);

				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}

				var loadedRecords = trafficRepo.Get(queryBuilder.Build());

				var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
				var resultDTO = _mapper.Map<List<TrafficResponseDTO>>(pagedRecords);
				return new PaginatedList<TrafficResponseDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
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

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder.WithInclude(
						//x => x.CampaignParticipation,
						x => x.CampaignParticipation.Campaign.Advertiser.ApplicationUser,
						x => x.CampaignParticipation.Publisher.ApplicationUser)
					.WithPredicate(x => x.CampaignParticipation.Campaign.Advertiser.ApplicationUser.Id == id);

				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}

				var loadedRecords = trafficRepo.Get(queryBuilder.Build());

				var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
				var resultDTO = _mapper.Map<List<TrafficResponseDTO>>(pagedRecords);
				return new PaginatedList<TrafficResponseDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
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

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder.WithInclude(
						//x => x.CampaignParticipation,
						x => x.CampaignParticipation.Campaign,
						x => x.CampaignParticipation.Publisher.ApplicationUser)
					.WithPredicate(x => x.CampaignParticipation.CampaignId == id);
				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}

				var loadedRecords = trafficRepo.Get(queryBuilder.Build());

				var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
				var resultDTO = _mapper.Map<List<TrafficResponseDTO>>(pagedRecords);
				return new PaginatedList<TrafficResponseDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
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
				var trafficClosedRepo = _unitOfWork.GetRepo<ClosedTraffic>();

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder
					.WithInclude(x => x.CampaignParticipation.Campaign)
					.WithPredicate(x =>
						(x.CampaignParticipation.Campaign.EndDate <= DateOnly.FromDateTime(DateTime.UtcNow)) ||
						(x.CampaignParticipation.Campaign.Status == CampaignStatus.Canceled) ||
						(x.CampaignParticipation.Campaign.Status == CampaignStatus.Completed)
					);

				var traffics = trafficRepo.Get(queryBuilder.Build());
				await trafficClosedRepo.CreateAllAsync(_mapper.Map<List<ClosedTraffic>>(traffics.ToList()));
				await _unitOfWork.SaveChangesAsync();
				await trafficRepo.DeleteAllAsync(traffics.ToList());
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
				var cPRepo = _unitOfWork.GetRepo<CampaignParticipation>();
				var ctRepo = _unitOfWork.GetRepo<ClosedTraffic>();
				var cRepo = _unitOfWork.GetRepo<Campaign>();
				var campaignList = await cPRepo.GetAllAsync(new QueryBuilder<CampaignParticipation>()
					.WithPredicate(x => x.PublisherId == publisherId)
					.Build()
					);

				var campaignIds = campaignList.Select(x => x.CampaignId).ToList();

				var campaigns = await cRepo.GetAllAsync(new QueryBuilder<Campaign>()
					.WithPredicate(x => campaignIds.Contains(x.Id) &&
										 (x.Status == CampaignStatus.Completed || x.Status == CampaignStatus.Canceled))
					.Build());

				var campaignParticipationIds = campaignList.Select(x => x.Id).ToList();
				var traffics = await ctRepo.GetAllAsync(new QueryBuilder<ClosedTraffic>()
					.WithPredicate(x => campaignParticipationIds.Contains((int)x.CampaignParticipationId))
					.Build());

				var trafficCountByCampaign = traffics.GroupBy(x => x.CampaignParticipationId).ToDictionary(g => g.Key, g => g.Count());

				var avgTraffic = campaignList
					.Where(x => campaigns.Any(c => c.Id == x.CampaignId))
					.Select(x => trafficCountByCampaign.GetValueOrDefault(x.Id, 0))
					.Average();

				return (int) avgTraffic;
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

				// Lấy campaign
				var campaign = await campaignRepo.GetSingleAsync(
					new QueryBuilder<Campaign>()
						.WithPredicate(x => x.Id == campaignId)
						.Build()
				);

				if (campaign == null)
				{
					throw new Exception($"Campaign with ID {campaignId} not found.");
				}

				var campaignParticipations = await cpRepo.GetAllAsync(
					new QueryBuilder<CampaignParticipation>()
						.WithPredicate(x => x.CampaignId == campaignId)
						.Build()
				);

				var campaignParticipationIds = campaignParticipations.Select(x => x.Id).ToList();

				if (!campaignParticipationIds.Any())
				{
					return 0;
				}

				if (campaign.Status == CampaignStatus.Activing ||
					campaign.Status == CampaignStatus.Pending ||
					campaign.Status == CampaignStatus.Approved)
				{
					var trafficRepo = _unitOfWork.GetRepo<Traffic>();

					var traffics = await trafficRepo.GetAllAsync(
						new QueryBuilder<Traffic>()
							.WithPredicate(x => campaignParticipationIds.Contains((int)x.CampaignParticipationId))
							.Build()
					);

					return traffics.Count();
				}
				else
				{
					var ctrafficRepo = _unitOfWork.GetRepo<ClosedTraffic>();

					var closedTraffics = await ctrafficRepo.GetAllAsync(
						new QueryBuilder<ClosedTraffic>()
							.WithPredicate(x => campaignParticipationIds.Contains((int)x.CampaignParticipationId))
							.Build()
					);

					return closedTraffics.Count();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

	}
}
