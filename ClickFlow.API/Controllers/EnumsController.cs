using ClickFlow.BLL.Helpers.Enum;
using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EnumsController : BaseAPIController
	{
		[Authorize]
		[HttpGet("campaign-status-list")]
		public IActionResult GetCampaignStatusList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<CampaignStatus>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[HttpGet("gender-list")]
		public IActionResult GetGenderList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<Gender>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[HttpGet("industry-list")]
		public IActionResult GetIndustryList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<Industry>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize]
		[HttpGet("report-status-list")]
		public IActionResult GetReportStatusList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<ReportStatus>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[HttpGet("role-list")]
		public IActionResult GetRoleList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<Role>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize]
		[HttpGet("type-pay-list")]
		public IActionResult GetTypePayList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<TypePay>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

		[Authorize]
		[HttpGet("transaction-type-list")]
		public IActionResult GetTransactionTypeList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<TransactionType>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
			}
		}

        [Authorize]
        [HttpGet("campaign-participation-status-list")]
        public IActionResult GetCampaignParticipationStatusList()
        {
            try
            {
                var list = EnumHelper.GetEnumList<CampaignParticipationStatus>();
                return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút nữa.");
            }
        }
    }
}
