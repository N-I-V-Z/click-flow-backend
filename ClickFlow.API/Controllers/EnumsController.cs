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
				return Error(ex.Message);
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
				return Error(ex.Message);
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
				return Error(ex.Message);
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
				return Error(ex.Message);
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
				return Error(ex.Message);
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
				return Error(ex.Message);
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
				return Error(ex.Message);
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
				return Error(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("message-type")]
		public IActionResult GetMessageTypeList()
		{
			try
			{
				var list = EnumHelper.GetEnumList<MessageType>();
				return list.Any() ? GetSuccess(list) : GetNotFound("Danh sách trống.");
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
