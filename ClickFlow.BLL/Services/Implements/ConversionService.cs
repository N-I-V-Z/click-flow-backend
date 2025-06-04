using AutoMapper;
using ClickFlow.BLL.DTOs.ConversionDTOs;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http.Extensions;

namespace ClickFlow.BLL.Services.Implements
{
	public class ConversionService : IConversionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ConversionService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		protected virtual QueryBuilder<Conversion> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<Conversion>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<Conversion>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<ConversionResponseDTO> CreateAsync(ConversionCreateDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Conversion>();

				// Kiểm tra trùng nếu OrderId được cung cấp
				if (!string.IsNullOrEmpty(dto.OrderId))
				{
					var existing = await repo.GetSingleAsync(new QueryBuilder<Conversion>()
						.WithPredicate(x =>
							x.ClickId == dto.ClickId &&
							x.OrderId == dto.OrderId
						).Build());

					if (existing != null)
						throw new Exception("Đã tồn tại Conversion với ClickId và OrderId này.");
				}

				var conversion = _mapper.Map<Conversion>(dto);
				conversion.Timestamp = DateTime.UtcNow;
				conversion.Status = ConversionStatus.Pending;

				await repo.CreateAsync(conversion);
				await _unitOfWork.SaveAsync();
				return _mapper.Map<ConversionResponseDTO>(conversion);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}


		public async Task<PaginatedList<ConversionResponseDTO>> GetAllAsync(ConversionGetAllDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Conversion>();
				var queryBuilder = CreateQueryBuilder(dto.Keyword);

				if (dto.Status != null)
				{
					queryBuilder.WithPredicate(x => x.Status == dto.Status);
				}

				if (dto.EventType != null)
				{
					queryBuilder.WithPredicate(x => x.EventType == dto.EventType);
				}

				if (!string.IsNullOrEmpty(dto.ClickId))
				{
					queryBuilder.WithPredicate(x => x.ClickId.Equals(dto.ClickId));
				}

				var conversions = repo.Get(queryBuilder.Build());

				var paged = await PaginatedList<Conversion>.CreateAsync(conversions, dto.PageIndex, dto.PageSize);
				var result = _mapper.Map<List<ConversionResponseDTO>>(paged);
				return new PaginatedList<ConversionResponseDTO>(result, paged.TotalItems, dto.PageIndex, dto.PageSize);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
			
		}

		public async Task<ConversionResponseDTO> GetByIdAsync(int id)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Conversion>();
				var queryBuilder = CreateQueryBuilder().WithPredicate(x => x.Id == id);

				var result = await repo.GetSingleAsync(queryBuilder.Build());
				return _mapper.Map<ConversionResponseDTO>(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
			
		}

		public async Task<ConversionResponseDTO> UpdateStatusAsync(int id, ConversionUpdateStatusDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Conversion>();
				var queryBuilder = CreateQueryBuilder().WithPredicate(x => x.Id == id);
				var conversion = await repo.GetSingleAsync(queryBuilder.Build());

				if (conversion == null)
					throw new Exception("Conversion not found");

				conversion.Status = dto.Status;
				await repo.UpdateAsync(conversion);
				await _unitOfWork.SaveAsync();
				return _mapper.Map<ConversionResponseDTO>(conversion);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
			
		}
	}
}
