using AutoMapper;
using ClickFlow.BLL.DTOs.ConversionDTOs;
using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class ConversionService : BaseServices<Conversion, ConversionResponseDTO>, IConversionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ConversionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ConversionResponseDTO> CreateAsync(ConversionCreateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var conversionRepo = _unitOfWork.GetRepo<Conversion>();
				var trafficRepo = _unitOfWork.GetRepo<Traffic>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				// 1) Kiểm tra trùng nếu OrderId được cung cấp
				if (!string.IsNullOrEmpty(dto.OrderId))
				{
					var existing = await conversionRepo.GetSingleAsync(new QueryBuilder<Conversion>()
						.WithPredicate(x =>
							x.ClickId == dto.ClickId &&
							x.OrderId == dto.OrderId
						).Build());

					if (existing != null)
						throw new Exception("Đã tồn tại Conversion với ClickId và OrderId này.");
				}

				// 2) Khởi tạo Conversion
				var conversion = _mapper.Map<Conversion>(dto);
				conversion.Timestamp = DateTime.UtcNow;
				conversion.Status = ConversionStatus.Pending;

				await conversionRepo.CreateAsync(conversion);
				await _unitOfWork.SaveAsync();

				// 3) Lấy Traffic từ ClickId để biết publisher
				var traffic = await trafficRepo.GetSingleAsync(new QueryBuilder<Traffic>()
					.WithPredicate(t => t.ClickId == dto.ClickId)
					.WithInclude(t => t.CampaignParticipation) // bao gồm CampaignParticipation để tìm Publisher
					.Build());

				if (traffic == null)
					throw new Exception($"Không tìm thấy Traffic với ClickId = {dto.ClickId}.");

				var publisherId = traffic.CampaignParticipation.PublisherId;

				// 4) Cộng tiền vào ví (nếu Conversion hợp lệ và có Revenue)
				if (conversion.Revenue.HasValue && conversion.Revenue.Value > 0)
				{
					var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
						.WithPredicate(w => w.UserId == publisherId)
						.Build());

					if (wallet == null)
						throw new Exception($"Ví của Publisher (ID={publisherId}) không tồn tại.");

					wallet.Balance += conversion.Revenue.Value;
					await walletRepo.UpdateAsync(wallet);

					// Ghi Transaction ghi nhận việc cộng tiền
					var transactionRepo = _unitOfWork.GetRepo<Transaction>();
					var tx = new Transaction
					{
						WalletId = wallet.Id,
						Amount = conversion.Revenue.Value,
						Balance = wallet.Balance,
						PaymentDate = DateTime.UtcNow,
						TransactionType = TransactionType.Withdraw,
					};
					await transactionRepo.CreateAsync(tx);
				}

				await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				return _mapper.Map<ConversionResponseDTO>(conversion);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				await _unitOfWork.RollBackAsync();
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

				return await GetPagedData(conversions, dto.PageIndex, dto.PageSize);
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

		public async Task<PublisherResponseDTO> GetPublisherIdByClickId(string clickId)
		{
			try
			{
				var conversionRepo = _unitOfWork.GetRepo<Conversion>();

				var queryBuilder = CreateQueryBuilder()
					.WithPredicate(x => x.ClickId.Equals(clickId))
					.WithInclude(x => x.Click.CampaignParticipation.Publisher);

				var conversion = await conversionRepo.GetSingleAsync(queryBuilder.Build());

				return _mapper.Map<PublisherResponseDTO>(conversion?.Click?.CampaignParticipation?.Publisher);
			}
			catch (Exception ex) {
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
