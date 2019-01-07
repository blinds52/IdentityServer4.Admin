using System;

namespace IdentityServer4.Admin.Rpc.Dtos
{
    public class PermissionCheckDto
    {
        public Guid UserId { get; set; }
        public string Permission { get; set; }
    }
}