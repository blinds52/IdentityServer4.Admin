using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class ClientController : ApiControllerBase
    {
        private readonly IDbContext _dbContext;

        public ClientController(IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateClientDto dto)
        {
            if (await _dbContext.Clients.AnyAsync(u => u.ClientId == dto.ClientId))
            {
                return new ApiResult(ApiResult.Error, "资源名已经存在");
            }

            var redirectUris = dto.RedirectUris.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(cors => !string.IsNullOrWhiteSpace(cors) && cors.IsUrl()).ToList();
            if (redirectUris.Count == 0)
            {
                return new ApiResult(ApiResult.Error, "回调地址不能为空");
            }

            var allowedCorsOrigins = dto.AllowedCorsOrigins.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(cors => !string.IsNullOrWhiteSpace(cors) && cors.IsUrl()).ToList();
            if (allowedCorsOrigins.Count == 0)
            {
                return new ApiResult(ApiResult.Error, "授权范围不能为空");
            }

            var client = new IdentityServer4.Models.Client();
            switch (dto.AllowedGrantTypes)
            {
                case GrantTypes.Code:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.Code;
                    break;
                }
                case GrantTypes.Hybrid:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.Hybrid;
                    break;
                }
                case GrantTypes.Implicit:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.Implicit;
                    break;
                }
                case GrantTypes.ClientCredentials:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.ClientCredentials;
                    break;
                }
                case GrantTypes.DeviceFlow:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.DeviceFlow;
                    break;
                }
                case GrantTypes.ResourceOwnerPassword:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.ResourceOwnerPassword;
                    break;
                }
                case GrantTypes.CodeAndClientCredentials:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.CodeAndClientCredentials;
                    break;
                }
                case GrantTypes.HybridAndClientCredentials:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.HybridAndClientCredentials;
                    break;
                }
                case GrantTypes.ImplicitAndClientCredentials:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.ImplicitAndClientCredentials;
                    break;
                }
                case GrantTypes.ResourceOwnerPasswordAndClientCredentials:
                {
                    client.AllowedGrantTypes = Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials;
                    break;
                }
            }

            client.Description = dto.Description;
            client.Properties = dto.Properties;
            client.AllowedScopes = dto.AllowedScopes.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                .Where(cors => !string.IsNullOrWhiteSpace(cors)).ToList();
            client.ClientId = dto.ClientId;
            client.ClientName = dto.ClientName;
            client.ClientUri = dto.ClientUri;
            client.ConsentLifetime = dto.ConsentLifetime;
            client.LogoUri = dto.LogoUri;
            client.ProtocolType = dto.ProtocolType;
            client.RedirectUris = redirectUris;
            client.RequireConsent = dto.RequireConsent;
            client.RequirePkce = dto.RequirePkce;
            client.AccessTokenLifetime = dto.AccessTokenLifetime;
            client.AccessTokenType = dto.AccessTokenType;
            client.AllowedCorsOrigins = allowedCorsOrigins;
            client.AllowOfflineAccess = dto.AllowOfflineAccess;
            client.AllowRememberConsent = dto.AllowRememberConsent;
            client.AuthorizationCodeLifetime = dto.AuthorizationCodeLifetime;
            client.ClientClaimsPrefix = dto.ClientClaimsPrefix;
            client.DeviceCodeLifetime = dto.DeviceCodeLifetime;
            client.EnableLocalLogin = dto.EnableLocalLogin;
            client.IdentityProviderRestrictions = dto.IdentityProviderRestrictions;
            client.IdentityTokenLifetime = dto.IdentityTokenLifetime;
            client.IncludeJwtId = dto.IncludeJwtId;
            client.RefreshTokenExpiration = dto.RefreshTokenExpiration;
            client.RefreshTokenUsage = dto.RefreshTokenUsage;
            client.RequireClientSecret = dto.RequireClientSecret;
            client.UserCodeType = dto.UserCodeType;
            client.UserSsoLifetime = dto.UserSsoLifetime;
            client.AbsoluteRefreshTokenLifetime = dto.AbsoluteRefreshTokenLifetime;
            client.AllowPlainTextPkce = dto.AllowPlainTextPkce;
            client.AlwaysSendClientClaims = dto.AlwaysSendClientClaims;
            client.BackChannelLogoutUri = dto.BackChannelLogoutUri;
            client.FrontChannelLogoutUri = dto.FrontChannelLogoutUri;
            client.PairWiseSubjectSalt = dto.PairWiseSubjectSalt;
            client.PostLogoutRedirectUris = dto.PostLogoutRedirectUris
                .Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(cors => !string.IsNullOrWhiteSpace(cors) && cors.IsUrl()).ToList();
            client.SlidingRefreshTokenLifetime = dto.SlidingRefreshTokenLifetime;
            client.AllowAccessTokensViaBrowser = dto.AllowAccessTokensViaBrowser;
            client.BackChannelLogoutSessionRequired = dto.BackChannelLogoutSessionRequired;
            client.FrontChannelLogoutSessionRequired = dto.FrontChannelLogoutSessionRequired;
            client.UpdateAccessTokenClaimsOnRefresh = dto.UpdateAccessTokenClaimsOnRefresh;
            client.AlwaysIncludeUserClaimsInIdToken = dto.AlwaysIncludeUserClaimsInIdToken;


            await _dbContext.Clients.AddAsync(client.ToEntity());
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAsync([FromQuery] PagedQuery input)
        {
            var queryResult = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .AsNoTracking()
                .PagedQuery(input);
            var dtos = new List<ClientDto>();
            foreach (var client in queryResult.Result)
            {
                dtos.Add(new ClientDto
                {
                    Id = client.Id,
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    AllowedGrantTypes = string.Join(" ", client.AllowedGrantTypes.Select(t => t.GrantType)),
                    AllowedScopes = string.Join(" ", client.AllowedScopes.Select(s => s.Scope))
                });
            }

            return new ApiResult(queryResult.ToResult(dtos));
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteAsync(int clientId)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(u => u.Id == clientId);
            if (client == null) return new ApiResult(ApiResult.Error, "客户端不存在或已经删除");

            _dbContext.Clients.Remove(client);
            // TODO: 确认其它关联表数据一并删除
            await _dbContext.SaveChangesAsync();
            return ApiResult.Ok;
        }
    }
}