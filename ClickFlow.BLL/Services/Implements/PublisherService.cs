using AutoMapper;
using ClickFlow.BLL.DTOs.PublisherDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PublisherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<PublisherResponseDTO>> GetAllPublishersAsync(int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<ApplicationUser>();

            var users = repo.Get(new QueryBuilder<ApplicationUser>()
                .WithPredicate(x => x.Role == Role.Publisher && !x.IsDeleted)
                .WithInclude(x => x.Publisher)
                .Build());

            var pagedUsers = await PaginatedList<ApplicationUser>.CreateAsync(users, pageIndex, pageSize);
            var result = _mapper.Map<List<PublisherResponseDTO>>(pagedUsers);
            return new PaginatedList<PublisherResponseDTO>(result, pagedUsers.TotalItems, pageIndex, pageSize);
        }
    }
}
