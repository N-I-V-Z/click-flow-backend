﻿using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.BLL.Services.Implements
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager,
                               SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockOutOnFailure)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockOutOnFailure);
            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
        {
            var otp = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
            return otp;
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            var existedUser = await _userManager.FindByEmailAsync(email);
            return existedUser;
        }

        public async Task<ApplicationUser> GetByEmailOrUserNameAsync(string input)
        {
            var user = await _userManager.FindByEmailAsync(input);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(input);
            }

            return user;
        }

        public async Task<ApplicationUser> GetByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user;
        }

        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles;
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            return user;
        }

        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<bool> IsLockedOutAsync(ApplicationUser user)
        {
            var response = await _userManager.IsLockedOutAsync(user);
            return response;
        }

        public async Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent, bool lockOutOnFailure)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockOutOnFailure);
            return result;
        }

        public async Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPass)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPass);
            return result;
        }

        public async Task<IdentityResult> SetTwoFactorEnabledAsync(ApplicationUser user, bool enable2Fa)
        {
            var result = await _userManager.SetTwoFactorEnabledAsync(user, enable2Fa);
            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
        {
            var signIn = await _signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);
            return signIn;
        }
    }
}
