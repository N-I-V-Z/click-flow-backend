using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IAdvertiserService
	{
		Task<Advertiser> GetAdvertiserByUserIdAsync(int userId);
	}
}
