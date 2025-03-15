using AutoMapper;
using ClickFlow.BLL.DTOs.PagingDTOs;
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

		public async Task<PaginatedList<TrafficViewDTO>> GetPagedData(IQueryable<Transaction> query, int pageIndex, int pageSize)
		{
			var paginatedEntities = await PaginatedList<Transaction>.CreateAsync(query, pageIndex, pageSize);
			var resultDto = _mapper.Map<List<TrafficViewDTO>>(paginatedEntities);

			return new PaginatedList<TrafficViewDTO>(resultDto, paginatedEntities.TotalItems, pageIndex, pageSize);
		}

		//public async Task<TrafficViewDTO> CreateAsync(TrafficCreateDTO dto)
		//{
		//	try
		//	{

		//		var trafficRepo = _unitOfWork.GetRepo<Traffic>();
		//		var campaignRepo = _unitOfWork.GetRepo<Campaign>();

		//		// traffic được tạo
		//		var newTraffic = _mapper.Map<Traffic>(dto);

		//		// campaign của traffic được tạo
		//		var campaign = await campaignRepo.GetSingleAsync(new QueryBuilder<Campaign>()
		//			.WithPredicate(x => x.Id == dto.CampaignId)
		//			.WithTracking(false)
		//			.Build()
		//			);

		//		// các traffic của campaign đó, để tính tổng tiền
		//		var trafficCampaign = await trafficRepo.GetAllAsync(new QueryBuilder<Traffic>()
		//			.WithPredicate(x => x.Id == dto.CampaignId && x.IsValid == true)
		//			.WithTracking(false)
		//			.Build()
		//			);
		//		var totalCommission = trafficCampaign.Sum(x => x.Revenue);
				
		//		// check xem có traffic nào cùng ip, cùng campaign, cùng publisher mà hợp lệ chưa
		//		var checkTrafficCPC = await trafficRepo.GetAllAsync(new QueryBuilder<Traffic>()
		//			.WithPredicate(x => x.IsValid == true &&
		//				x.IpAddress == dto.IpAddress &&
		//				x.CampaignId == dto.CampaignId &&
		//				x.PublisherId == dto.PublisherId)
		//			.WithTracking(false)
		//			.Build());

		//		// ngày hiện tại
		//		var currentDay = new DateOnly();

				

		//		if (campaign.EndDate < currentDay || campaign.StartDate > currentDay)
		//		{
		//			newTraffic.IsValid = false;
		//		}
		//		else if (campaign.TypePay == TypePay.CPC && checkTrafficCPC.Any())
		//		{
		//			newTraffic.IsValid = false;
		//		}
		//		else if (campaign.TypePay == TypePay.CPC && totalCommission + campaign.Commission < campaign.Budget)
		//		{
		//			newTraffic.IsValid = false;
		//		}
		//		else
		//		{
		//			newTraffic.IsValid = true;
		//		}

		//		if (newTraffic.IsValid == true)
		//		{
		//			if (campaign.TypePay == TypePay.CPC)
		//			{
		//				newTraffic.Revenue = campaign.Commission;
		//			}
		//		}


		//		await trafficRepo.CreateAsync(newTraffic);
		//		var saver = await _unitOfWork.SaveAsync();
		//		if (!saver)
		//		{
		//			return null;
		//		}


		//		return _mapper.Map<TrafficViewDTO>(newTraffic);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex.ToString());
		//		throw;
		//	}
		//}

		public async Task<PaginatedList<TrafficViewDTO>> GetAllAsync(PagingRequestDTO dto)
		{
			try
			{
				var trafficRepo = _unitOfWork.GetRepo<Traffic>();

				var queryBuilder = CreateQueryBuilder();
				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}
				var loadedRecords = trafficRepo.Get(queryBuilder.Build());

				var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
				var resultDTO = _mapper.Map<List<TrafficViewDTO>>(pagedRecords);
				return new PaginatedList<TrafficViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<TrafficViewDTO> GetByIdAsync(int id)
		{
			try
			{
				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder.WithPredicate(x => x.Id == id);

				var trafficRepo = _unitOfWork.GetRepo<Traffic>();
				var response = await trafficRepo.GetSingleAsync(queryOptions.Build());
				if (response == null) return null;
				return _mapper.Map<TrafficViewDTO>(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		//public async Task<PaginatedList<TrafficViewDTO>> GetAllByPublisherIdAsync(int id, PagingRequestDTO dto)
		//{
		//	try
		//	{
		//		var trafficRepo = _unitOfWork.GetRepo<Traffic>();

		//		var queryBuilder = CreateQueryBuilder();
		//		var queryOptions = queryBuilder.WithPredicate(x => x.PublisherId == id);
		//		if (!string.IsNullOrEmpty(dto.Keyword))
		//		{
		//			var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
		//			queryBuilder.WithPredicate(predicate);
		//		}

		//		var loadedRecords = trafficRepo.Get(queryBuilder.Build());

		//		var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
		//		var resultDTO = _mapper.Map<List<TrafficViewDTO>>(pagedRecords);
		//		return new PaginatedList<TrafficViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex.ToString());
		//		throw;
		//	}
		//}

		//public async Task<PaginatedList<TrafficViewDTO>> GetAllByAdvertiserIdAsync(int id, PagingRequestDTO dto)
		//{
		//	try
		//	{
		//		var trafficRepo = _unitOfWork.GetRepo<Traffic>();

		//		var queryBuilder = CreateQueryBuilder();
		//		var queryOptions = queryBuilder.WithInclude(x => x.Campaign).WithPredicate(x => x.Campaign.AdvertiserId == id);
		//		if (!string.IsNullOrEmpty(dto.Keyword))
		//		{
		//			var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
		//			queryBuilder.WithPredicate(predicate);
		//		}

		//		var loadedRecords = trafficRepo.Get(queryBuilder.Build());

		//		var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
		//		var resultDTO = _mapper.Map<List<TrafficViewDTO>>(pagedRecords);
		//		return new PaginatedList<TrafficViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex.ToString());
		//		throw;
		//	}
		//}

  //      public async Task<PaginatedList<TrafficViewDTO>> GetAllByCampaignIdAsync(int id, PagingRequestDTO dto)
  //      {
  //          try
  //          {
  //              var trafficRepo = _unitOfWork.GetRepo<Traffic>();

  //              var queryBuilder = CreateQueryBuilder();
  //              var queryOptions = queryBuilder.WithPredicate(x => x.CampaignId == id);
  //              if (!string.IsNullOrEmpty(dto.Keyword))
  //              {
  //                  var predicate = FilterHelper.BuildSearchExpression<Traffic>(dto.Keyword);
  //                  queryBuilder.WithPredicate(predicate);
  //              }

  //              var loadedRecords = trafficRepo.Get(queryBuilder.Build());

  //              var pagedRecords = await PaginatedList<Traffic>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
  //              var resultDTO = _mapper.Map<List<TrafficViewDTO>>(pagedRecords);
  //              return new PaginatedList<TrafficViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
  //          }
  //          catch (Exception ex)
  //          {
  //              Console.WriteLine(ex.ToString());
  //              throw;
  //          }
  //      }
    }
}
