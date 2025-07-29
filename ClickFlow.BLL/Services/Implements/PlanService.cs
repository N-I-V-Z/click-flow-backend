using AutoMapper;
using ClickFlow.BLL.DTOs.PlanDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class PlanService : BaseServices<Plan, PlanResponseDTO>, IPlanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PlanService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<PaginatedList<PlanResponseDTO>> GetAllAsync(PlanGetAllDTO dto)
		{
			var repo = _unitOfWork.GetRepo<Plan>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword);

			if (dto.IsActive != null)
			{
				queryBuilder.WithPredicate(x => x.IsActive == dto.IsActive);
			}

			var loadedRecords = repo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<PlanResponseDTO> GetByIdAsync(int id)
		{
			var planRepo = _unitOfWork.GetRepo<Plan>();
			var entity = await planRepo.GetSingleAsync(
				new QueryBuilder<Plan>()
					.WithPredicate(p => p.Id == id)
					.Build());

			if (entity == null)
				throw new KeyNotFoundException($"Plan with ID {id} not found.");

			return _mapper.Map<PlanResponseDTO>(entity);
		}

		public async Task<PlanResponseDTO> CreateAsync(PlanCreateDTO dto)
		{
			var planRepo = _unitOfWork.GetRepo<Plan>();

			var entity = _mapper.Map<Plan>(dto);
			entity.IsActive = true;

			await planRepo.CreateAsync(entity);
			await _unitOfWork.SaveAsync();

			return _mapper.Map<PlanResponseDTO>(entity);

		}

		public async Task<PlanResponseDTO> UpdateAsync(int id, PlanUpdateDTO dto)
		{
			var planRepo = _unitOfWork.GetRepo<Plan>();
			var entity = await planRepo.GetSingleAsync(
				CreateQueryBuilder()
					.WithPredicate(p => p.Id == id)
					.Build());

			if (entity == null)
				throw new KeyNotFoundException($"Plan with ID {id} not found.");

			_mapper.Map(dto, entity);

			await planRepo.UpdateAsync(entity);
			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<PlanResponseDTO>(entity);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var planRepo = _unitOfWork.GetRepo<Plan>();
			var entity = await planRepo.GetSingleAsync(
				new QueryBuilder<Plan>()
					.WithPredicate(p => p.Id == id)
					.Build());

			if (entity == null)
				throw new KeyNotFoundException("Không tìm thấy Plan.");

			entity.IsActive = false;
			await planRepo.UpdateAsync(entity);
			return await _unitOfWork.SaveAsync();
		}
	}
}
