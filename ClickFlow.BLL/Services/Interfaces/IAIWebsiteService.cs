namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IAIWebsiteService
	{
		Task<string> GetWebsiteAIResponseAsync(int userId, string question);
	}
}
