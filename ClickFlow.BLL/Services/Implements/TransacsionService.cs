using AutoMapper;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
namespace ClickFlow.BLL.Services.Implements
{
	public class TransacsionService : BaseServices<Transaction, TransactionResponseDTO>, ITransactionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		//private readonly PayOS _payOS;

		public TransacsionService(IUnitOfWork unitOfWork, IMapper mapper/*, PayOS payOS */) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			//_payOS = payOS;
		}

		public async Task<TransactionResponseDTO> CreateTransactionAsync(int userId, TransactionCreateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				// 1) Lấy Wallet bằng dto.WalletId
				var wallet = await walletRepo.GetSingleAsync(
					new QueryBuilder<Wallet>()
						.WithPredicate(x => x.UserId == userId)
						.WithTracking(false)
						.Build());

				if (wallet == null)
					throw new Exception("Ví không tồn tại.");

				// 2) Khởi tạo đối tượng Transaction (pendding)
				var newTransaction = _mapper.Map<Transaction>(dto);
				newTransaction.PaymentDate = DateTime.UtcNow;
				newTransaction.Status = false; // pending
				newTransaction.Balance = wallet.Balance; // lưu số dư hiện tại
				newTransaction.WalletId = wallet.Id;

				// 3) Tùy loại TransactionType
				if (dto.TransactionType == TransactionType.Withdraw)
				{
					// Chỉ kiểm tra số dư tại thời điểm tạo request
					if (wallet.Balance < dto.Amount)
						throw new Exception("Số dư trong ví không đủ để thực hiện rút tiền.");

					// Khi pending, không trừ tiền ngay
					// newTransaction.Balance = wallet.Balance; // đã gán ở trên
				}
				else if (dto.TransactionType == TransactionType.Deposit)
				{
					// Pending deposit, không cộng tiền ngay
					// newTransaction.Balance = wallet.Balance; // đã gán
				}
				else
				{
					throw new Exception("TransactionType không hợp lệ.");
				}

				// 4) Lưu Transaction pending
				var created = await transactionRepo.CreateAsync(newTransaction);
				await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				return _mapper.Map<TransactionResponseDTO>(created);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}


		public async Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsByUserIdAsync(int userId, PagingRequestDTO dto)
		{
			var walletRepo = _unitOfWork.GetRepo<Wallet>();
			var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
				.WithPredicate(x => x.UserId == userId)
				.WithTracking(false)
				.Build());

			if (wallet == null)
			{
				// Nếu không có ví, trả danh sách rỗng
				return new PaginatedList<TransactionResponseDTO>(
					Enumerable.Empty<TransactionResponseDTO>().ToList(),
					0, dto.PageIndex, dto.PageSize);
			}

			var queryBuilder = CreateQueryBuilder(dto.Keyword);
			var queryOptions = queryBuilder.WithPredicate(x => x.WalletId == wallet.Id).WithOrderBy(x => x.OrderByDescending(x => x.PaymentDate));

			var transactionRepo = _unitOfWork.GetRepo<Transaction>();
			var transactions = transactionRepo.Get(queryOptions.Build());

			return await GetPagedData(transactions, dto.PageIndex, dto.PageSize);
		}

		public async Task<TransactionResponseDTO> UpdateStatusTransactionAsync(long id, TransactionUpdateStatusDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder.WithPredicate(x => x.Id == id);

				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				var transaction = await transactionRepo.GetSingleAsync(queryOptions.Build());

				if (transaction == null)
					throw new Exception("Transaction không tồn tại.");

				if (transaction.Status == dto.Status)
					throw new Exception("Không thay đổi trạng thái.");

				var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
					.WithPredicate(x => x.Id == transaction.WalletId)
					.WithTracking(false)
					.Build()
					);

				if (wallet == null)
					throw new Exception("Ví không tồn tại.");

				transaction.Status = dto.Status;

				if (dto.Status == true)
				{
					if (transaction.TransactionType == TransactionType.Deposit)
					{
						wallet.Balance += transaction.Amount;
						transaction.Balance = wallet.Balance;
					}
					else if (transaction.TransactionType == TransactionType.Withdraw)
					{
						if (wallet.Balance < transaction.Amount)
							throw new Exception("Số dư trong ví không đủ để rút.");

						wallet.Balance -= transaction.Amount;
						transaction.Balance = wallet.Balance;
					}
				}
				await transactionRepo.UpdateAsync(transaction);
				await walletRepo.UpdateAsync(wallet);
				await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				return _mapper.Map<TransactionResponseDTO>(transaction);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<PaginatedList<TransactionResponseDTO>> GetAllTransactionsAsync(PagingRequestDTO dto)
		{
			var queryBuilder = CreateQueryBuilder(dto.Keyword).WithOrderBy(x => x.OrderByDescending(x => x.PaymentDate));

			var transactionRepo = _unitOfWork.GetRepo<Transaction>();
			var transactions = transactionRepo.Get(queryBuilder.Build());

			return await GetPagedData(transactions, dto.PageIndex, dto.PageSize);
		}
	}
}