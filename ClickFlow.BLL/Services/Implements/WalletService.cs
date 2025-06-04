using AutoMapper;
using ClickFlow.BLL.DTOs.WalletDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class WalletService : BaseServices<Wallet, WalletResponseDTO>, IWalletService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public WalletService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<WalletResponseDTO> CreateWalletAsync(int userId, WalletCreateDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Wallet>();

				var createdWallet = _mapper.Map<Wallet>(dto);
				createdWallet.UserId = userId;

				await repo.CreateAsync(createdWallet);

				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return null;
				}

				return _mapper.Map<WalletResponseDTO>(createdWallet);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<WalletResponseDTO> GetWalletByUserIdAsync(int id)
		{
			try
			{
				var walletRepo = _unitOfWork.GetRepo<Wallet>();

				var walletQueryBuilder = CreateQueryBuilder();
				var walletQueryOptions = walletQueryBuilder
					.WithPredicate(x => x.UserId == id)
					.Build();
				var wallet = await walletRepo.GetSingleAsync(walletQueryOptions);

				return _mapper.Map<WalletResponseDTO>(wallet);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<WalletResponseDTO> UpdateWalletAsync(int id, WalletUpdateDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Wallet>();

				var updatedWallet = new Wallet
				{
					Id = id,
					Balance = dto.Balance
				};

				await repo.UpdateAsync(updatedWallet);

				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return null;
				}

				return _mapper.Map<WalletResponseDTO>(updatedWallet);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
