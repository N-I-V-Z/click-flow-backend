using AutoMapper;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.ReportDTOs;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        protected virtual QueryBuilder<Report> CreateQueryBuilder(string? search = null)
        {
            var queryBuilder = new QueryBuilder<Report>()
                                .WithTracking(false);

            if (!string.IsNullOrEmpty(search))
            {
                var predicate = FilterHelper.BuildSearchExpression<Report>(search);
                queryBuilder.WithPredicate(predicate);
            }

            return queryBuilder;
        }

        public async Task<PaginatedList<ReportViewDTO>> GetPagedData(IQueryable<Report> query, int pageIndex, int pageSize)
        {
            var paginatedEntities = await PaginatedList<Report>.CreateAsync(query, pageIndex, pageSize);
            var resultDto = _mapper.Map<List<ReportViewDTO>>(paginatedEntities);

            return new PaginatedList<ReportViewDTO>(resultDto, paginatedEntities.TotalItems, pageIndex, pageSize);
        }

        public async Task<ReportViewDTO> CreateReportAsync(int advertiserId, ReportCreateDTO dto)
        {
            try
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

                return _mapper.Map<ReportViewDTO>(newReport);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<PaginatedList<ReportViewDTO>> GetAllAsync(PagingRequestDTO dto)
        {
            try
            {
                var trafficRepo = _unitOfWork.GetRepo<Report>();

                var queryBuilder = CreateQueryBuilder().WithInclude(x => x.Campaign, x => x.Advertiser, x => x.Publisher, x => x.Publisher.ApplicationUser, x => x.Advertiser.ApplicationUser);
                if (!string.IsNullOrEmpty(dto.Keyword))
                {
                    var predicate = FilterHelper.BuildSearchExpression<Report>(dto.Keyword);
                    queryBuilder.WithPredicate(predicate);
                }
                var loadedRecords = trafficRepo.Get(queryBuilder.Build());

                var pagedRecords = await PaginatedList<Report>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
                var resultDTO = _mapper.Map<List<ReportViewDTO>>(pagedRecords);
                return new PaginatedList<ReportViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ReportViewDTO> GetByIdAsync(int id)
        {
            try
            {
                var trafficRepo = _unitOfWork.GetRepo<Report>();
                var queryBuilder = CreateQueryBuilder()
                    .WithPredicate(x => x.Id == id)
                    .WithInclude(
                        x => x.Campaign,
                        x => x.Advertiser,
                        x => x.Publisher,
                        x => x.Publisher.ApplicationUser,
                        x => x.Advertiser.ApplicationUser);

                var queryOptions = queryBuilder.Build();

                var response = await trafficRepo.GetSingleAsync(queryOptions);
                if (response == null) return null;
                return _mapper.Map<ReportViewDTO>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ReportViewDTO> UpdateResponseReportAsync(int id, string response)
        {
            try
            {
                var trafficRepo = _unitOfWork.GetRepo<Report>();
                var traffic = await trafficRepo.GetSingleAsync(new QueryBuilder<Report>()
                                                        .WithPredicate(x => x.Id == id)
                                                        .WithTracking(false)
                                                        .Build());
                traffic.Response = response;
                var saver = await _unitOfWork.SaveAsync();
                if (!saver)
                {
                    return null;
                }

                return _mapper.Map<ReportViewDTO>(traffic);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ReportViewDTO> UpdateStatusReportAsync(int id, ReportStatus status)
        {
            try
            {
                var trafficRepo = _unitOfWork.GetRepo<Report>();
                var response = await trafficRepo.GetSingleAsync(new QueryBuilder<Report>()
                                                        .WithPredicate(x => x.Id == id)
                                                        .WithTracking(false)
                                                        .Build());
                response.Status = status;
                var saver = await _unitOfWork.SaveAsync();
                if (!saver)
                {
                    return null;
                }

                return _mapper.Map<ReportViewDTO>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<PaginatedList<ReportViewDTO>> GetByStatusAsync(ReportsGetByStatusDTO dto)
        {
            try
            {
                var trafficRepo = _unitOfWork.GetRepo<Report>();

                var queryBuilder = CreateQueryBuilder()
                    .WithInclude(
                        x => x.Campaign,
                        x => x.Advertiser,
                        x => x.Publisher,
                        x => x.Publisher.ApplicationUser,
                        x => x.Advertiser.ApplicationUser)
                    .WithPredicate(x => x.Status == dto.Status);

                if (!string.IsNullOrEmpty(dto.Keyword))
                {
                    var predicate = FilterHelper.BuildSearchExpression<Report>(dto.Keyword);
                    queryBuilder.WithPredicate(predicate);
                }
                var loadedRecords = trafficRepo.Get(queryBuilder.Build());

                var pagedRecords = await PaginatedList<Report>.CreateAsync(loadedRecords, dto.PageIndex, dto.PageSize);
                var resultDTO = _mapper.Map<List<ReportViewDTO>>(pagedRecords);
                return new PaginatedList<ReportViewDTO>(resultDTO, pagedRecords.TotalItems, dto.PageIndex, dto.PageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
