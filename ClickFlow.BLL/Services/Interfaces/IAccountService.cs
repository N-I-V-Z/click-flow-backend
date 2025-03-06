using ClickFlow.BLL.DTOs.AccountDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest);
        Task<AuthenResultDTO> SignInAsync(AuthenDTO authenDTO);
        Task<AuthenResultDTO> GenerateTokenAsync(ApplicationUser user);
        Task<BaseResponse> SendEmailConfirmation(ApplicationUser user);
        Task<BaseResponse> SendOTP2FA(ApplicationUser user, string password);
        Task<BaseResponse> SignOutAsync(SignOutDTO signOutDTO);
        Task<BaseResponse> CheckToRenewTokenAsync(AuthenResultDTO authenResult);
        Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto);
        Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto);
        Task<BaseResponse> SendOTPByEmailAsync(string email);
    }
}
