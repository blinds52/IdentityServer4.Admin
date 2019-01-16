using System;
using System.Collections.Generic;
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
            logger.LogInformation($"ServiceId: {req.ServiceId}, MessageId: {req.MessageId}, Data: {message}");

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var options = serviceProvider.GetRequiredService<AdminOptions>();

            if (!options.AllowAnonymousUserQuery)
            {
                return rsp;
            }

            var ids = JsonConvert.DeserializeObject<Guid[]>(message);
            var users = await userManager.Users.Where(u => ids.Contains(u.Id)).OrderByDescending(u => u.CreationTime)
                .Select(u => new
                {
                    u.Id,
                    Name = u.FirstName + u.LastName,
                    u.Title,
                    u.Email,
                    Mobile = u.PhoneNumber,
                    u.OfficePhone,
                    u.Group,
                    u.Level,
                    u.UserName,
                    u.CreationTime
                }).ToListAsync();
            rsp.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(users));
            return rsp;
        }

        private async Task<AmpMessage> SearchUsersAsync(IServiceProvider serviceProvider, AmpMessage req)
        {
            var rsp = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            var message = Encoding.UTF8.GetString(req.Data);
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UserService>();
            logger.LogInformation($"ServiceId: {req.ServiceId}, MessageId: {req.MessageId}, Data: {message}");
            var dbContext = serviceProvider.GetRequiredService<IDbContext>();
            var options = serviceProvider.GetRequiredService<AdminOptions>();

            if (!options.AllowAnonymousUserQuery)
            {
                return rsp;
            }

            var dto = JsonConvert.DeserializeObject<PagedQueryUserDto>(message);
            Expression<Func<User, bool>> where = null;
            dto.Q = string.IsNullOrWhiteSpace(dto.Q) ? null : dto.Q.Trim();
            dto.Group = string.IsNullOrWhiteSpace(dto.Group) ? null : dto.Group.Trim();
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

            if (dto.Roles == null || dto.Roles.Length == 0)
            {
                var output = await dbContext.Users.OrderByDescending(u => u.CreationTime).PagedQueryAsync(dto, where);
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
                        u.Level,
                        u.UserName
                    }).ToList()
                };
                rsp.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            }
            else
            {
                var roleIds = await dbContext.Roles.Where(r => dto.Roles.Contains(r.Name)).Select(r => r.Id)
                    .ToListAsync();

                var result = await dbContext.Users.OrderByDescending(u => u.CreationTime).Join(dbContext.UserRoles,
                        u => u.Id, ur => ur.UserId, (u, ur) =>
                            new TmpUser
                            {
                                Id = u.Id,
                                Name = u.FirstName + u.LastName,
                                Title = u.Title,
                                Email = u.Email,
                                Mobile = u.PhoneNumber,
                                OfficePhone = u.OfficePhone,
                                Group = u.Group,
                                Level = u.Level,
                                RoleId = ur.RoleId,
                                UserName = u.UserName,
                                CreationTime = u.CreationTime
                            }).Where(t =>
                        roleIds.Contains(t.RoleId) &&
                        (string.IsNullOrWhiteSpace(dto.Q) || t.Name.Contains(dto.Q) || t.Email.Contains(dto.Q) ||
                         t.Mobile.Contains(dto.Q))
                        && (string.IsNullOrWhiteSpace(dto.Group) || t.Group == dto.Group)
                        && (dto.Titles == null || dto.Titles.Length == 0 || dto.Titles.Contains(t.Title))).Distinct()
                    .PagedQueryAsync(dto);
                var output = new PagedQueryResult
                {
                    Total = result.Total,
                    Size = result.Size,
                    Page = result.Page,
                    Result = result.Result.Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Title,
                        r.Email,
                        r.Mobile,
                        r.OfficePhone,
                        r.Group,
                        r.Level,
                        r.UserName
                    })
                };
                rsp.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(output));
            }

            return rsp;
        }

        class TmpUser : IEqualityComparer<TmpUser>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string OfficePhone { get; set; }
            public string Group { get; set; }
            public string Level { get; set; }
            public Guid RoleId { get; set; }
            public string UserName { get; set; }
            public DateTime CreationTime { get; set; }

            public bool Equals(TmpUser x, TmpUser y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(TmpUser obj)
            {
                return Id.GetHashCode();
            }
        }
    }
}