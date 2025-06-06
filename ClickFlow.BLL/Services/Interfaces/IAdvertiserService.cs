using ClickFlow.BLL.DTOs.AdvertiserDTOs;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IAdvertiserService
	{
        Task<Advertiser> GetAdvertiserByUserIdAsync(int userId);
    }
}
