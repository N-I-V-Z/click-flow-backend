using AutoMapper;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class AdvertiserService : IAdvertiserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AdvertiserService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<Advertiser> GetAdvertiserByUserIdAsync(int userId)
		{
			var advertiserRepo = _unitOfWork.GetRepo<Advertiser>();

			var advertiser = await advertiserRepo.GetSingleAsync(new QueryBuilder<Advertiser>()
				.WithPredicate(a => a.UserId == userId)
				.Build());

			return advertiser;
		}
	}
}
