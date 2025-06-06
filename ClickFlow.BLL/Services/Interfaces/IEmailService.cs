using ClickFlow.BLL.DTOs.EmailDTOs;
using ClickFlow.BLL.DTOs.Response;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IEmailService
	{
		public BaseResponse SendEmail(EmailDTO emailDTO);
	}
}
