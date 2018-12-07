using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using IdentityServer4.Admin.Repositories;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class DashboardController : ApiControllerBase
    {
        private readonly IRepository<User, Guid> _userRepository;
        private readonly ClientRepository _clientRepository;
        private readonly ApiResourceRepository _apiResourceRepository;
        
        public DashboardController(IRepository<User, Guid> userRepository,
            ClientRepository  clientRepository,
            ApiResourceRepository apiResourceRepository,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory) : base(unitOfWork, loggerFactory)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _apiResourceRepository = apiResourceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] PaginationQuery input)
        {
            var output = new DashboardDto();
            output.ApiResourceCount = await _apiResourceRepository.CountAsync();
            output.ClientCount = await _clientRepository.CountAsync();
            output.LockedUserCount = await _userRepository.CountAsync(u => u.LockoutEnd < DateTime.Now);
            output.UserCount = await _userRepository.CountAsync();
            return new ApiResult(output);
        }
    }
}