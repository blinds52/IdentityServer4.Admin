using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/user")]
    [Authorize]
    [SecurityHeaders]
    public class ProfileController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public ProfileController(UserManager<User> userManager,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _userManager = userManager;
            _serviceProvider = serviceProvider;
        }

        [Route("{userId}/profile")]
        public async Task<IActionResult> GetProfileAsync(Guid userId)
        {
            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            var dto = new UserOutputDto();
            if (user != null)
            {
                dto.PhoneNumber = user.PhoneNumber;
                dto.Email = user.Email;
                dto.UserName = user.UserName;
                dto.Id = user.Id;
                dto.Sex = user.Sex;
                dto.Group = user.Group;
                dto.Title = user.Title;
                dto.FirstName = user.FirstName;
                dto.LastName = user.LastName;
                dto.OfficePhone = user.OfficePhone;
                dto.Level = user.Level;
                dto.Roles = string.Join("; ", await _userManager.GetRolesAsync(user));
            }

            return new ApiResult(dto);
        }

        [HttpPut("{userId}/profile")]
        public async Task<IActionResult> UpdateAsync(Guid userId, [FromBody] UpdateProfileInputDto dto)
        {
            dto.Email = dto.Email.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.UserName = dto.UserName.Trim();
            dto.FirstName = dto.FirstName?.Trim();
            dto.LastName = dto.LastName?.Trim();
            dto.OfficePhone = dto.OfficePhone?.Trim();

            if (User.FindFirst("sub")?.Value != userId.ToString())
                return new ApiResult(ApiResult.Error, "验证失败");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName && dto.UserName != AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户名不能修改");

            if (user.UserName != AdminConsts.AdminName && dto.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"用户名不能是 {AdminConsts.AdminName}");

            string normalizedName =
                _serviceProvider.ProtectPersonalData(_userManager.NormalizeKey(dto.UserName),
                    _userManager.Options);
            // NormalizedUserName 有唯一索引, 应该用它做查询
            // TODO: 判断邮件、用户名是否有重复
            if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedName
                                                       && u.Id != userId))
            {
                return new ApiResult(ApiResult.Error, "用户名已经存在");
            }

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.OfficePhone = dto.OfficePhone;
            user.Sex = dto.Sex;
            
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpPut("{userId}/password")]
        public async Task<IActionResult> ChangePasswordAsync(Guid userId, [FromBody] ChangeSelfPasswordInputDto dto)
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