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
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName )
                return new ApiResult(ApiResult.Error, "验证失败");
           
            user.Email = dto.Email.Trim();
            user.PhoneNumber = dto.PhoneNumber.Trim();
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpPut("{userId}/password")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangeSelfPasswordDto dto)
        {
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword.Trim(), dto.NewPassword.Trim());
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }
    }
}