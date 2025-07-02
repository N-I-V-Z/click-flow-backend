using AutoMapper;
using ClickFlow.BLL.DTOs.ReportDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class ReportService : BaseServices<Report, ReportResponseDTO>, IReportService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ReportService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ReportResponseDTO> CreateReportAsync(int advertiserId, ReportCreateDTO dto)
		{
			var reportRepo = _unitOfWork.GetRepo<Report>();
			var newReport = _mapper.Map<Report>(dto);

			newReport.AdvertiserId = advertiserId;
			newReport.Status = ReportStatus.Pending;
			newReport.CreateAt = DateTime.UtcNow;

			await reportRepo.CreateAsync(newReport);

			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<ReportResponseDTO>(newReport);
		}

		public async Task<PaginatedList<ReportResponseDTO>> GetAllAsync(ReportGetAllDTO dto)
		{
			var trafficRepo = _unitOfWork.GetRepo<Report>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword).WithInclude(
				x => x.Campaign,
				//x => x.Advertiser, 
				//x => x.Publisher, 
				x => x.Publisher.ApplicationUser,
				x => x.Advertiser.ApplicationUser);

			if (dto.Status != null)
			{
				queryBuilder.WithPredicate(x => x.Status == dto.Status);
			}

			var loadedRecords = trafficRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<ReportResponseDTO> GetByIdAsync(int id)
		{
			var trafficRepo = _unitOfWork.GetRepo<Report>();
			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(x => x.Id == id)
				.WithInclude(
					x => x.Campaign,
					//x => x.Advertiser,
					//x => x.Publisher,
					x => x.Publisher.ApplicationUser,
					x => x.Advertiser.ApplicationUser);

			var queryOptions = queryBuilder.Build();

			var response = await trafficRepo.GetSingleAsync(queryOptions);
			if (response == null) return null;
			return _mapper.Map<ReportResponseDTO>(response);
		}

		public async Task<ReportResponseDTO> UpdateResponseReportAsync(int id, string response)
		{
			var reportRepo = _unitOfWork.GetRepo<Report>();
			var report = await reportRepo.GetSingleAsync(new QueryBuilder<Report>()
													.WithPredicate(x => x.Id == id)
													.WithTracking(false)
													.Build());
			report.Response = response;
			await reportRepo.UpdateAsync(report);
			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<ReportResponseDTO>(report);
		}

		public async Task<ReportResponseDTO> UpdateStatusReportAsync(int id, ReportStatus status)
		{
			var reportRepo = _unitOfWork.GetRepo<Report>();
			var report = await reportRepo.GetSingleAsync(new QueryBuilder<Report>()
													.WithPredicate(x => x.Id == id)
													.WithTracking(false)
													.Build());
			report.Status = status;
			await reportRepo.UpdateAsync(report);
			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<ReportResponseDTO>(report);
		}
	}
}
