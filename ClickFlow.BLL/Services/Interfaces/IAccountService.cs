using ClickFlow.BLL.DTOs.AccountDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.DAL.Entities;

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
        Task<AuthenResultDTO> GenerateTokenFromRefreshTokenAsync(AuthenResultDTO authenResult);
        Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto);
        Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto);
        Task<BaseResponse> SendOTPByEmailAsync(string email);
        Task<BaseResponse> UpdateUserBlockStatusAsync(int userId, bool isBlocked);
        Task<AuthenResultDTO> SignInWithGoogleAsync(GoogleAuthDTO dto);
    }
}
