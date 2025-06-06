using ClickFlow.BLL.Helpers.Validations;
using ClickFlow.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClickFlow.BLL.DTOs.AccountDTOs
{
	public class AccountCreateRequestDTO
	{
		[Required(ErrorMessage = "Email không được bỏ trống")]
		[EmailAddress(ErrorMessage = "Email sai định dạng. Định dạng đúng: example@gmail.com")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Nhắc lại mật khẩu không được để trống")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Họ và tên không được để trống")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Số điện thoại không được để trống")]
		[Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Role không được để trống")]
		public Role Role { get; set; }

		// Thông tin riêng cho Advertiser
		[RequiredIfRole(Role.Advertiser, "CompanyName")]
		public string? CompanyName { get; set; }

		[RequiredIfRole(Role.Advertiser, "IntroductionWebsite")]
		public string? IntroductionWebsite { get; set; }

		[RequiredIfRole(Role.Advertiser, "StaffSize")]
		public int? StaffSize { get; set; }

		[RequiredIfRole(Role.Advertiser, "Industry")]
		public Industry? Industry { get; set; }
	}
}
