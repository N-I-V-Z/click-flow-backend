﻿using AutoMapper;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.TransactionDTOs;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
namespace ClickFlow.BLL.Services.Implements
{
	public class TransacsionService : ITransactionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public TransacsionService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		protected virtual QueryBuilder<Transaction> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<Transaction>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<Transaction>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<PaginatedList<TransactionResponseDTO>> GetPagedData(IQueryable<Transaction> query, int pageIndex, int pageSize)
		{
			var paginatedEntities = await PaginatedList<Transaction>.CreateAsync(query, pageIndex, pageSize);
			var resultDto = _mapper.Map<List<TransactionResponseDTO>>(paginatedEntities);

			return new PaginatedList<TransactionResponseDTO>(resultDto, paginatedEntities.TotalItems, pageIndex, pageSize);
		}
		public async Task<TransactionResponseDTO> CreateTransactionAsync(TransactionCreateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
					.WithPredicate(x => x.Id == dto.WalletId)
					.WithTracking(false)
					.Build()
					);

				Transaction newTransaction = null;
				var createdTransaction = _mapper.Map<Transaction>(dto);
				createdTransaction.PaymentDate = DateTime.UtcNow;

				if (dto.TransactionType == TransactionType.Withdraw)
				{
					if (wallet.Balance < dto.Amount)
					{
						throw new Exception("Số dư trong ví không đủ để rút.");
					}

					if (dto.Status == true)
						createdTransaction.Balance = wallet.Balance - dto.Amount;

					newTransaction = await transactionRepo.CreateAsync(createdTransaction);
					await _unitOfWork.SaveChangesAsync();

					//if (dto.Status == true)
					//{
					//	wallet.Balance -= dto.Amount;
					//	await walletRepo.UpdateAsync(wallet);
					//}

				}
				else if (dto.TransactionType == TransactionType.Deposit)
				{
					createdTransaction.Balance = wallet.Balance;

					newTransaction = await transactionRepo.CreateAsync(createdTransaction);
				}
				else
				{
					throw new Exception("TransactionType không hợp lệ.");
				}
				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				if (!saver)
				{
					return null;
				}

				return _mapper.Map<TransactionResponseDTO>(newTransaction);
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
			try
			{
				var walletRepo = _unitOfWork.GetRepo<Wallet>();
				var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
					.WithPredicate(x => x.UserId == userId)
					.WithTracking(false)
					.Build());

				var queryBuilder = CreateQueryBuilder(dto.Keyword);
				var queryOptions = queryBuilder.WithPredicate(x => x.WalletId == wallet.Id);
				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Transaction>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}
				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var transactions = transactionRepo.Get(queryOptions.Build());

				var results = _mapper.Map<List<TransactionResponseDTO>>(transactions);

				var pageResults = await GetPagedData(transactions, dto.PageIndex, dto.PageSize);

				return pageResults;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<TransactionResponseDTO> UpdateStatusTransactionAsync(int id, TransactionUpdateStatusDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var queryBuilder = CreateQueryBuilder();
				var queryOptions = queryBuilder.WithPredicate(x => x.Id == id);

				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				var transaction = await transactionRepo.GetSingleAsync(queryOptions.Build());

				if (transaction.Status == dto.Status) throw new Exception("Không thay đổi");

				var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
					.WithPredicate(x => x.Id == transaction.WalletId)
					.WithTracking(false)
					.Build()
					);

				transaction.Status = dto.Status;

				if (dto.Status == true)
				{
					if (transaction.TransactionType == TransactionType.Deposit)
					{
						wallet.Balance += transaction.Amount;
						transaction.Balance = wallet.Balance + transaction.Amount;
					}
					else if (transaction.TransactionType == TransactionType.Withdraw)
					{
						if (wallet.Balance < transaction.Amount) throw new Exception("Số tiền trong ví không đủ để rút");
						wallet.Balance -= transaction.Amount;
						transaction.Balance = wallet.Balance - transaction.Amount;
					}
					await transactionRepo.UpdateAsync(transaction);
					await _unitOfWork.SaveChangesAsync();
					await walletRepo.UpdateAsync(wallet);
					await _unitOfWork.SaveChangesAsync();
				}
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
			try
			{
				var queryBuilder = CreateQueryBuilder(dto.Keyword);
				var queryOptions = queryBuilder;

				if (!string.IsNullOrEmpty(dto.Keyword))
				{
					var predicate = FilterHelper.BuildSearchExpression<Transaction>(dto.Keyword);
					queryBuilder.WithPredicate(predicate);
				}
				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var transactions = transactionRepo.Get(queryOptions.Build());

				var results = _mapper.Map<List<TransactionResponseDTO>>(transactions);

				var pageResults = await GetPagedData(transactions, dto.PageIndex, dto.PageSize);

				return pageResults;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}