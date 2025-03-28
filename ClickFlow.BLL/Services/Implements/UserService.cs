﻿using AutoMapper;
using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ApplicationUserResponseDTO>> GetUsersByRoleAsync(Role role, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<ApplicationUser>();

            var users = repo.Get(new QueryBuilder<ApplicationUser>()
                .WithPredicate(x => x.Role == role && !x.IsDeleted)
                .WithInclude(x => x.Advertiser)
                .Build());

            var pagedUsers = await PaginatedList<ApplicationUser>.CreateAsync(users, pageIndex, pageSize);
            var result = _mapper.Map<List<ApplicationUserResponseDTO>>(pagedUsers);
            return new PaginatedList<ApplicationUserResponseDTO>(result, pagedUsers.TotalItems, pageIndex, pageSize);
        }
    }
}

