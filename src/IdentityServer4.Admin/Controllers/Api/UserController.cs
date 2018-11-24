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

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
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
                dto.UserName = user.Email;
                dto.Id = user.Id;
                dto.Roles = string.Join(",", await _userManager.GetRolesAsync(user));
            }

            return new ApiResult(dto);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Update(string userId, UserDto dto)
        {
            if (dto.Id != userId) return new ApiResult(ApiResult.Error, "更新编码号不匹配");

            var result = await _userManager.UpdateAsync(new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Id = dto.Id
            });
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
                PhoneNumber = dto.Phone
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