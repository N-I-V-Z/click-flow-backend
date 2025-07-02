using AutoMapper;
using ClickFlow.BLL.DTOs.CourseDTOs;
using ClickFlow.BLL.DTOs.PagingDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class CourseService : BaseServices<Course, CourseResponseDTO>, ICourseService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public CourseService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<bool> CheckPublisherInCourseAsync(int publisherId, int courseId)
		{
			var cpRepo = _unitOfWork.GetRepo<CoursePublisher>();
			return await cpRepo.AnyAsync(new QueryBuilder<CoursePublisher>().WithPredicate(x => x.PublisherId == publisherId && x.CourseId == courseId).Build());

		}

		public async Task<CourseResponseDTO> CreateCourseAsync(int userId, CourseCreateDTO dto)
		{
			var courseRepo = _unitOfWork.GetRepo<Course>();
			var newCourse = _mapper.Map<Course>(dto);

			newCourse.CreateAt = DateTime.UtcNow;
			newCourse.CreateById = userId;

			await courseRepo.CreateAsync(newCourse);

			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}

			return _mapper.Map<CourseResponseDTO>(newCourse);
		}

		public async Task<PaginatedList<CourseResponseDTO>> GetAllCourseForPublisherAsync(int publisherId, PagingRequestDTO dto)
		{
			var repo = _unitOfWork.GetRepo<Course>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword)
				.WithInclude(x => x.CoursePublishers)
				.WithPredicate(x =>
					x.CoursePublishers.FirstOrDefault(c => c.PublisherId == publisherId) == null
				);

			var loadedRecords = repo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<PaginatedList<CourseResponseDTO>> GetAllCoursesAsync(PagingRequestDTO dto)
		{
			var courseRepo = _unitOfWork.GetRepo<Course>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword);

			var loadedRecords = courseRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<CourseResponseDTO> GetCourseByIdAsync(int id)
		{
			var courseRepo = _unitOfWork.GetRepo<Course>();
			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(x => x.Id == id);

			var queryOptions = queryBuilder.Build();

			var response = await courseRepo.GetSingleAsync(queryOptions);
			if (response == null) return null;
			return _mapper.Map<CourseResponseDTO>(response);
		}

		public async Task<PaginatedList<CourseResponseDTO>> GetJoinedCoursesAsync(int userId, PagingRequestDTO dto)
		{
			var courseRepo = _unitOfWork.GetRepo<Course>();

			var queryBuilder = CreateQueryBuilder(dto.Keyword)
				.WithInclude(x => x.CoursePublishers)
				.WithPredicate(x => x.CoursePublishers.FirstOrDefault(x => x.PublisherId == userId) != null);

			var loadedRecords = courseRepo.Get(queryBuilder.Build());

			return await GetPagedData(loadedRecords, dto.PageIndex, dto.PageSize);
		}

		public async Task<BaseResponse> JoinTheCourseAsync(int courseId, int publisherId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var cpRepo = _unitOfWork.GetRepo<CoursePublisher>();
				var existing = await cpRepo.GetSingleAsync(
					new QueryBuilder<CoursePublisher>()
						.WithPredicate(x => x.CourseId == courseId && x.PublisherId == publisherId)
						.Build());

				if (existing != null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Bạn đã tham gia khóa học này rồi" };
				}

				var courseRepo = _unitOfWork.GetRepo<Course>();
				var course = await courseRepo.GetSingleAsync(
					CreateQueryBuilder()
						.WithPredicate(x => x.Id == courseId)
						.Build());

				if (course == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Khóa học không tồn tại" };
				}

				var walletRepo = _unitOfWork.GetRepo<Wallet>();
				var wallet = await walletRepo.GetSingleAsync(
					new QueryBuilder<Wallet>()
						.WithPredicate(x => x.UserId == publisherId)
						.Build());

				if (wallet == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy ví người dùng" };
				}

				if (wallet.Balance < course.Price)
				{
					return new BaseResponse { IsSuccess = false, Message = "Số dư không đủ để tham gia khóa học" };
				}

				// Trừ tiền
				wallet.Balance -= course.Price;
				await walletRepo.UpdateAsync(wallet);

				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				var newTransaction = new Transaction
				{
					Amount = course.Price,
					Balance = wallet.Balance - course.Price,
					PaymentDate = DateTime.UtcNow,
					Status = true,
					TransactionType = TransactionType.Pay,
					WalletId = wallet.Id
				};
				await transactionRepo.CreateAsync(newTransaction);
				await _unitOfWork.SaveChangesAsync();

				var userRepo = _unitOfWork.GetRepo<ApplicationUser>();
				var admin = await userRepo.GetSingleAsync(new QueryBuilder<ApplicationUser>().WithPredicate(x => x.Role == Role.Admin).Build());
				var adminWallet = await walletRepo.GetSingleAsync(new QueryBuilder<Wallet>().WithPredicate(x => x.UserId == admin.Id).Build());
				adminWallet.Balance += course.Price;

				var adminTransaction = new Transaction
				{
					Amount = course.Price,
					Balance = wallet.Balance,
					PaymentDate = DateTime.UtcNow,
					Status = true,
					TransactionType = TransactionType.Received,
					WalletId = adminWallet.Id
				};

				await _unitOfWork.SaveChangesAsync();

				// Tham gia khóa học
				var coursePublisher = new CoursePublisher
				{
					CourseId = courseId,
					PublisherId = publisherId,
				};

				await cpRepo.CreateAsync(coursePublisher);

				var success = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				return new BaseResponse { IsSuccess = success };
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollBackAsync();
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<BaseResponse> RateTheCourseAsync(int courseId, int publisherId, CourseRateDTO dto)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var cpRepo = _unitOfWork.GetRepo<CoursePublisher>();

				var cp = await cpRepo.GetSingleAsync(new QueryBuilder<CoursePublisher>().WithPredicate(x => x.CourseId == courseId && x.PublisherId == publisherId && x.Rate == null).Build());

				if (cp == null) return new BaseResponse { IsSuccess = false, Message = "Người dùng chưa tham gia khóa học hoặc đã đánh giá khóa học" };

				cp.Rate = dto.Rate;
				await cpRepo.UpdateAsync(cp);
				await _unitOfWork.SaveChangesAsync();

				var ratedList = await cpRepo.GetAllAsync(
					new QueryBuilder<CoursePublisher>()
						.WithPredicate(x => x.CourseId == courseId && x.Rate != null)
						.Build());
				var avgRate = ratedList.Average(x => x.Rate.Value);

				var courseRepo = _unitOfWork.GetRepo<Course>();
				var course = await courseRepo.GetSingleAsync(
					CreateQueryBuilder()
						.WithPredicate(x => x.Id == courseId)
						.Build());

				if (course != null)
				{
					course.AvgRate = avgRate;
					await courseRepo.UpdateAsync(course);
				}

				var saver = await _unitOfWork.SaveAsync();
				await _unitOfWork.CommitTransactionAsync();

				if (!saver)
				{
					return null;
				}

				return new BaseResponse
				{
					IsSuccess = saver
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<CourseResponseDTO> UpdateCourseAsync(int id, CourseUpdateDTO dto)
		{
			try
			{
				var repo = _unitOfWork.GetRepo<Course>();

				var course = await repo.GetSingleAsync(CreateQueryBuilder()
					.WithPredicate(x => x.Id == id)
					.WithTracking(false)
					.Build());

				if (course == null)
				{
					return null;
				}

				_mapper.Map(dto, course);
				await repo.UpdateAsync(course);

				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return null;
				}

				return _mapper.Map<CourseResponseDTO>(course);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}
	}
}
