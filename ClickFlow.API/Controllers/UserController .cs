using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseAPIController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("by-role/{role}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetUsersByRole([FromRoute] Role role, [FromRoute] int pageIndex, [FromRoute] int pageSize)
        {
            try
            {
                var response = await _userService.GetUsersByRoleAsync(role, pageIndex, pageSize);            
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau ít phút.");
            }
        }
    }
}
