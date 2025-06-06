using AutoMapper;
using ClickFlow.BLL.DTOs.UserPlanDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class UserPlanService : BaseServices<UserPlan, UserPlanResponseDTO>, IUserPlanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public UserPlanService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<UserPlanResponseDTO> GetCurrentPlanAsync(int publisherId)
		{
			try
			{
				var upRepo = _unitOfWork.GetRepo<UserPlan>();
				var userPlan = await upRepo.GetSingleAsync(
					new QueryBuilder<UserPlan>()
						.WithPredicate(up => up.UserId == publisherId)
						.WithInclude(up => up.Plan)
						.Build());

				if (userPlan == null)
					throw new KeyNotFoundException($"Publisher (ID={publisherId}) chưa có gói.");

				// Nếu ExpirationDate != null và đã qua ngày → hết hạn
				if (userPlan.ExpirationDate != null && DateTime.UtcNow > userPlan.ExpirationDate.Value)
					throw new Exception("Gói của bạn đã hết hạn, vui lòng gia hạn.");

				return _mapper.Map<UserPlanResponseDTO>(userPlan);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<UserPlanResponseDTO> AssignPlanToPublisherAsync(int userId, int planId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				// 1) Lấy Plan
				var planRepo = _unitOfWork.GetRepo<Plan>();
				var targetPlan = await planRepo.GetSingleAsync(
					new QueryBuilder<Plan>()
						.WithPredicate(p => p.Id == planId && p.IsActive)
						.Build());
				if (targetPlan == null)
					throw new KeyNotFoundException($"Plan (ID={planId}) không tồn tại hoặc không active.");

				// 2) Kiểm tra Wallet (Balance)
				var walletRepo = _unitOfWork.GetRepo<Wallet>();
				var wallet = await walletRepo.GetSingleAsync(
					new QueryBuilder<Wallet>().WithPredicate(w => w.UserId == userId).Build());
				if (wallet == null)
					throw new KeyNotFoundException($"Ví của Publisher (ID={userId}) không tồn tại.");

				if (wallet.Balance < targetPlan.Price)
					throw new Exception($"Số dư ví không đủ. Bạn cần {targetPlan.Price} để đăng ký gói này.");

				// 3) Trừ tiền vào Ví
				// Nếu Price thực tế có thập phân, cân nhắc đổi Wallet.Balance thành decimal
				wallet.Balance -= (int)targetPlan.Price;
				await walletRepo.UpdateAsync(wallet);

				// 4) Tạo Transaction ghi nhận thanh toán
				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var tx = new Transaction
				{
					WalletId = wallet.Id,
					Amount = (int)targetPlan.Price,
					Balance = wallet.Balance,
					PaymentDate = DateTime.UtcNow,
					TransactionType = TransactionType.Pay,
				};
				await transactionRepo.CreateAsync(tx);

				// 5) Tính ExpirationDate dựa vào DurationDays (int?):
				//    - null  => vĩnh viễn
				//    - >0    => hết hạn sau X ngày
				//    - 0     => cũng coi là vĩnh viễn
				DateTime? expirationDate = null;
				if (targetPlan.DurationDays.HasValue)
				{
					int days = targetPlan.DurationDays.Value;
					if (days > 0)
						expirationDate = DateTime.UtcNow.AddDays(days);
				}

				// 6) Lấy hoặc tạo mới UserPlan
				var upRepo = _unitOfWork.GetRepo<UserPlan>();
				var existingUserPlan = await upRepo.GetSingleAsync(
					new QueryBuilder<UserPlan>().WithPredicate(up => up.UserId == userId).Build());

				if (existingUserPlan != null)
				{
					// Cập nhật gói (reset counters, StartDate, ExpirationDate)
					existingUserPlan.PlanId = planId;
					existingUserPlan.StartDate = DateTime.UtcNow;
					existingUserPlan.ExpirationDate = expirationDate;
					existingUserPlan.CurrentClicks = 0;
					existingUserPlan.CurrentConversions = 0;
					existingUserPlan.CurrentCampaigns = 0;

					await upRepo.UpdateAsync(existingUserPlan);
				}
				else
				{
					// Tạo mới UserPlan
					var newUserPlan = new UserPlan
					{
						UserId = userId,
						PlanId = planId,
						StartDate = DateTime.UtcNow,
						ExpirationDate = expirationDate,
						CurrentClicks = 0,
						CurrentConversions = 0,
						CurrentCampaigns = 0
					};
					await upRepo.CreateAsync(newUserPlan);
				}

				// 7) Commit transaction sau cùng
				await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				// Trả về gói hiện tại đã gán (gọi lại GetCurrentPlanAsync)
				return await GetCurrentPlanAsync(userId);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<bool> CanAddCampaignAsync(int publisherId)
		{
			try
			{
				var userPlan = await GetCurrentPlanAsync(publisherId);
				return userPlan.CurrentCampaigns < userPlan.Plan.MaxCampaigns;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<bool> IncreaseClickCountAsync(int publisherId)
		{
			try
			{
				var upRepo = _unitOfWork.GetRepo<UserPlan>();
				var userPlanEntity = await upRepo.GetSingleAsync(
					new QueryBuilder<UserPlan>()
						.WithPredicate(up => up.UserId == publisherId)
						.WithInclude(up => up.Plan)
						.Build());

				if (userPlanEntity == null)
					throw new KeyNotFoundException($"Publisher (ID={publisherId}) chưa được gán gói nào.");

				// Kiểm tra hết hạn và hạn mức
				if (userPlanEntity.ExpirationDate != null && DateTime.UtcNow > userPlanEntity.ExpirationDate.Value)
					return false;

				if (userPlanEntity.CurrentClicks >= userPlanEntity.Plan.MaxClicksPerMonth)
					return false;

				userPlanEntity.CurrentClicks++;
				await upRepo.UpdateAsync(userPlanEntity);
				await _unitOfWork.SaveAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<bool> IncreaseConversionCountAsync(int publisherId)
		{
			try
			{
				var upRepo = _unitOfWork.GetRepo<UserPlan>();
				var userPlanEntity = await upRepo.GetSingleAsync(
					new QueryBuilder<UserPlan>()
						.WithPredicate(up => up.UserId == publisherId)
						.WithInclude(up => up.Plan)
						.Build());

				if (userPlanEntity == null)
					throw new KeyNotFoundException($"Publisher (ID={publisherId}) chưa được gán gói nào.");

				// Kiểm tra hết hạn và hạn mức
				if (userPlanEntity.ExpirationDate != null && DateTime.UtcNow > userPlanEntity.ExpirationDate.Value)
					return false;

				if (userPlanEntity.CurrentConversions >= userPlanEntity.Plan.MaxConversionsPerMonth)
					return false;

				userPlanEntity.CurrentConversions++;
				await upRepo.UpdateAsync(userPlanEntity);
				await _unitOfWork.SaveAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
