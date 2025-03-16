using AutoMapper;
using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Paging;
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

    }
}
