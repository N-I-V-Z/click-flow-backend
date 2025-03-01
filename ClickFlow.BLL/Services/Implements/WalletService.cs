using AutoMapper;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.WalletDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using System.Net;

namespace ClickFlow.BLL.Services.Implements
{
	public class WalletService : IWalletService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public WalletService(IUnitOfWork unitOfWork, IMapper mapper) 
		{ 
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<WalletViewDTO> CreateWalletAsync(WalletCreateDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Wallet>();
				var createdWallet = _mapper.Map<Wallet>(dto);

				await repo.CreateAsync(createdWallet);

				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return null;
				}

				return _mapper.Map<WalletViewDTO>(createdWallet);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<WalletViewDTO> GetWalletByUserIdAsync(int id)
		{
			try
			{
				var userRepo = _unitOfWork.GetRepo<User>();
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				var user = await userRepo.GetSingleAsync(new QueryBuilder<User>()
					.WithPredicate(x => x.Id == id)
					.WithTracking(false)
					.WithInclude(x => x.Wallet)
					.Build()
					);

				var wallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>()
					.WithPredicate(x => x.Id == user.WalletId)
					.WithTracking(false)
					.Build()
					);

				return _mapper.Map<WalletViewDTO>(wallet);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<WalletViewDTO> UpdateWalletAsync(int id, WalletUpdateDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Wallet>();
				var updatedWallet = new Wallet { 
					Id = id,
					Balance = dto.Balance
				};

				await repo.UpdateAsync(updatedWallet);

				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return null;
				}

				return _mapper.Map<WalletViewDTO>(updatedWallet);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
