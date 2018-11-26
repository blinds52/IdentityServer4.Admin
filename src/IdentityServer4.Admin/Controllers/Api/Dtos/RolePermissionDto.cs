namespace IdentityServer4.Admin.Controllers.Api.Dtos
{
    public class RolePermissionDto
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string Permission { get; set; }
    }
}