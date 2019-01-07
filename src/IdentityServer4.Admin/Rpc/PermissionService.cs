using System;
using System.Text;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Rpc.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IdentityServer4.Admin.Rpc
{
    public class PermissionService : ServiceActor
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<AmpMessage> ProcessAsync(AmpMessage req)
        {
            var rsp = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
            var message = Encoding.UTF8.GetString(req.Data);
            var logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UserService>();
            logger.LogInformation($"Message: {message}");

            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = _serviceProvider.GetRequiredService<IDbContext>();

            var dto = JsonConvert.DeserializeObject<PermissionCheckDto>(message);
            var key = $"{dto.UserId}_{dto.Permission}";

            var isGrant =
                await dbContext.UserPermissionKeys.AnyAsync(up => up.Permission == key);

            rsp.Data = Encoding.UTF8.GetBytes(isGrant ? "true" : "false");
            return rsp;
        }

        protected override int ServiceId => 10001;
    }
}