using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/user")]
    [Authorize]
    [SecurityHeaders]
    public class ProfileController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AdminDbContext _dbContext;

        public ProfileController(UserManager<User> userManager, AdminDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [Route("{userId}/profile")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            var dto = new UserDto();
            if (user != null)
            {
                dto.PhoneNumber = user.PhoneNumber;
                dto.Email = user.Email;
                dto.UserName = user.UserName;
                dto.Id = user.Id;
                dto.Roles = string.Join(",", await _userManager.GetRolesAsync(user));
            }

            return new ApiResult(dto);
        }

        [HttpPut("{userId}/profile")]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateProfileDto dto)
        {
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.Email = dto.Email.Trim();

            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "验证失败");

            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpPut("{userId}/password")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangeSelfPasswordDto dto)
        {            
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");
            
            dto.NewPassword = dto.NewPassword.Trim();
            dto.OldPassword = dto.OldPassword?.Trim();
            
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }
    }
}