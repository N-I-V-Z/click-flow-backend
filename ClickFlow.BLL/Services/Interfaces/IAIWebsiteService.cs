namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IAIWebsiteService
	{
		Task<string> GetWebsiteAIResponseAsync(string question);
	}
}
