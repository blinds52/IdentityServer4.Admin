using System.Collections.Generic;
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
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class UserController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AdminDbContext _dbContext;

        public UserController(UserManager<User> userManager, AdminDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        #region User

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            dto.Password = dto.Password.Trim();
            dto.Email = dto.Email.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.UserName = dto.UserName.Trim();

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

            return new ApiResult(ApiResult.Error,
                string.Join(",", result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        public async Task<IActionResult> Find([FromQuery] PaginationQuery input)
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
                    IsDelete = user.IsDeleted,
                    Roles = string.Join(",", await _userManager.GetRolesAsync(user))
                });
            }

            output.Result = userDtos;
            return new ApiResult(output);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> FindFirst(int userId)
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
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserDto dto)
        {
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.Email = dto.Email.Trim();
            dto.UserName = dto.UserName.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName && dto.UserName != AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户名不能修改");

            if (user.UserName != AdminConsts.AdminName && dto.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, $"用户名不能是: {AdminConsts.AdminName}");

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted == false);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在或已经删除");
            if (user.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户不能删除");
            user.IsDeleted = true;
            user.LockoutEnabled = true;
            await _userManager.UpdateAsync(user);
            return ApiResult.Ok;
        }

        [HttpPut("{userId}/changePassword")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            if (user.UserName == AdminConsts.AdminName) return new ApiResult(ApiResult.Error, "管理员密码请通过 Profile 入口修改");
            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded) return new ApiResult(ApiResult.Error, result.Errors.First().Description);

            result = await _userManager.AddPasswordAsync(user, dto.NewPassword.Trim());
            return result.Succeeded ? ApiResult.Ok : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion

        #region User Permission

        [HttpGet("{userId}/permission")]
        public IActionResult FindRolePermission([FromQuery] PaginationQuery input)
        {
            var output = _dbContext.UserPermissions.PageList(input);
            var userPermissions = (IEnumerable<UserPermission>) output.Result;
            var userPermissionDtos = new List<UserPermissionDto>();
            foreach (var userPermission in userPermissions)
            {
                userPermissionDtos.Add(new UserPermissionDto
                {
                    PermissionId = userPermission.PermissionId,
                    Permission = userPermission.Permission,
                    UserId = userPermission.UserId,
                    Id = userPermission.Id
                });
            }

            output.Result = userPermissionDtos;
            return new ApiResult(output);
        }

        [HttpPost("{userId}/permission/{permissionId}")]
        public async Task<IActionResult> CreateUserPermission(int userId, int permissionId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");

            if (user.UserName == AdminConsts.AdminName)
                return new ApiResult(ApiResult.Error, "管理员用户不能添加权限");

            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId);
            if (permission == null) return new ApiResult(ApiResult.Error, "权限不存在");

            if (await _dbContext.UserPermissions.AnyAsync(
                ur => ur.UserId == userId && ur.PermissionId == permissionId))
                return new ApiResult(ApiResult.Error, "权限已经存在");

            await _dbContext.UserPermissions.AddAsync(new UserPermission
            {
                Permission = permission.Name,
                UserId = userId,
                PermissionId = permissionId
            });
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpDelete("{userId}/permission/{permissionId}")]
        public async Task<IActionResult> DeleteUserPermission(int userId, int permissionId)
        {
            var userPermission =
                await _dbContext.UserPermissions.FirstOrDefaultAsync(p =>
                    p.UserId == userId && p.PermissionId == permissionId);
            if (userPermission == null) return new ApiResult(ApiResult.Error, "数据不存在");
            _dbContext.UserPermissions.Remove(userPermission);
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        #endregion

        #region User Role

        [HttpPost("{userId}/role/{roleId}")]
        public async Task<IActionResult> CreateUserRole(int userId, int roleId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");

            if (await _userManager.IsInRoleAsync(user, AdminConsts.AdminName))
                return new ApiResult(ApiResult.Error, "管理员不能添加角色");

            var role = await _dbContext.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            return result.Succeeded
                ? ApiResult.Ok
                : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        [HttpGet("{userId}/role")]
        public async Task<IActionResult> FindUserRole(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");
            var roleIds = _dbContext.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId);
            var roles = _dbContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });

            return new ApiResult(roles);
        }

        [HttpDelete("{userId}/role/{roleId}")]
        public async Task<IActionResult> DeleteUserRole(int userId, int roleId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return new ApiResult(ApiResult.Error, "用户不存在");

            var role = await _dbContext.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null) return new ApiResult(ApiResult.Error, "角色不存在");

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return result.Succeeded
                ? ApiResult.Ok
                : new ApiResult(ApiResult.Error, result.Errors.First().Description);
        }

        #endregion
    }
}