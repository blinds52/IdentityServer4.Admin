using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityServer4.Admin.Rpc
{
    public class UserService : ServiceActor
    {
        /// <summary>
        /// 服务的标识
        /// </summary>
        protected override int ServiceId => 10000;

        private readonly IServiceProvider _serviceProvider;

        public UserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 处理消息请求
        /// </summary>
        /// <param name="req">请求的消息</param>
        /// <returns>返回消息</returns>
        public override async Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            AmpMessage rsp;
            var serviceProvider = _serviceProvider.CreateScope().ServiceProvider;
            switch (req.MessageId)
            {
                // 查询
                case 0:
                {
                    rsp = await SearchUsersAsync(serviceProvider, req);
                    break;
                }
                case 1:
                {
                    rsp = await GetUsersAsync(serviceProvider, req);
                    break;
                }
                default:
                {
                    rsp = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
                    break;
                }
            }

            return rsp;
        }

        private async Task<AmpMessage> GetUsersAsync(IServiceProvider serviceProvider, AmpMessage req)
        {
            var rsp = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            var message = Encoding.UTF8.GetString(req.Data);
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UserService>();
            logger.LogInformation($"Message: {message}");

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var options = serviceProvider.GetRequiredService<IOptions<AdminOptions>>().Value;

            if (!options.AllowAnonymousUserQuery)
            {
                return rsp;
            }

            var ids = JsonConvert.DeserializeObject<Guid[]>(message);
            var users = await userManager.Users.Where(u => ids.Contains(u.Id)).Select(u => new
            {
                u.Id,
                Name = u.FirstName + u.LastName,
                u.Title,
                u.Email,
                Mobile = u.PhoneNumber,
                u.OfficePhone,
                u.Group,
                u.Level
            }).ToListAsync();
            rsp.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(users));
            return rsp;
        }

        private async Task<AmpMessage> SearchUsersAsync(IServiceProvider serviceProvider, AmpMessage req)
        {
            var rsp = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            var message = Encoding.UTF8.GetString(req.Data);
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UserService>();
            logger.LogInformation($"Message: {message}");
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var options = serviceProvider.GetRequiredService<IOptions<AdminOptions>>().Value;

            if (!options.AllowAnonymousUserQuery)
            {
                return rsp;
            }

            var dto = JsonConvert.DeserializeObject<PagedQueryUserDto>(message);
            Expression<Func<User, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(dto.Q))
            {
                where = u =>
                    (u.FirstName + u.LastName).Contains(dto.Q) || u.Email.Contains(dto.Q) ||
                    u.PhoneNumber.Contains(dto.Q);
            }

            if (!string.IsNullOrWhiteSpace(dto.Group))
            {
                if (where == null)
                {
                    where = u => u.Group == dto.Group.Trim();
                }
                else
                {
                    where = where.AndAlso(u => u.Group == dto.Group.Trim());
                }
            }

            if (dto.Titles != null && dto.Titles.Length > 0)
            {
                if (where == null)
                {
                    where = u => dto.Titles.Contains(u.Title);
                }
                else
                {
                    where = where.AndAlso(u => dto.Titles.Contains(u.Title));
                }
            }

            var output = await userManager.Users.PagedQuery(dto, where);
            var result = new PagedQueryResult
            {
                Page = output.Page,
                Size = output.Size,
                Total = output.Total,
                Result = output.Result.Select(u => new
                {
                    u.Id,
                    Name = u.FirstName + u.LastName,
                    u.Title,
                    u.Email,
                    Mobile = u.PhoneNumber,
                    u.OfficePhone,
                    u.Group,
                    u.Level
                }).ToList()
            };
            rsp.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            return rsp;
        }
    }
}