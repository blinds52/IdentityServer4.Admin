using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Controllers.Api.Dtos;
using IdentityServer4.Admin.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserStore<User> _userStore;

        public UserController(UserManager<User> userManager, IUserStore<User> userStore,
            IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _userStore = userStore;
        }

        [Authorize(Roles = "super-admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null)
            {
                return new ApiResult(ApiResult.Error, "用户不存在");
            }

            await _userManager.DeleteAsync(user);
            return ApiResult.Ok;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFirst(string userId)
        {
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

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpPut("{userId}/changePassword")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResult(ApiResult.Error, result.Errors.First().Description);
            }

            result = await _userManager.AddPasswordAsync(user, dto.NewPassword);

            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (await _userManager.Users.AnyAsync(u => u.UserName == dto.UserName && u.IsDeleted == false))
            {
                return new ApiResult(ApiResult.Error, "用户名已经存在");
            }

            var result = await _userManager.CreateAsync(new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            }, dto.Password);
            if (result.Succeeded)
            {
                return ApiResult.Ok;
            }
            else
            {
                return new ApiResult(ApiResult.Error,
                    string.Join(",", result.Errors.Select(e => e.Description)));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] PaginationQuery input)
        {
            var output = _userManager.Users.PageList(input, u => u.IsDeleted == false);
            var users = (IEnumerable<User>) output.Result;
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = string.Join(",", await _userManager.GetRolesAsync(user))
                });
            }

            output.Result = userDtos;
            return new ApiResult(output);
        }
    }
}