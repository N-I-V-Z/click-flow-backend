using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdvertiserController : BaseAPIController
	{
		private readonly IAdvertiserService _advertiserService;
		private readonly IConfiguration _configuration;

		public AdvertiserController(IAdvertiserService advertiserService, IConfiguration configuration)
		{
			_advertiserService = advertiserService;
			this._configuration = configuration;
		}
	}
}
