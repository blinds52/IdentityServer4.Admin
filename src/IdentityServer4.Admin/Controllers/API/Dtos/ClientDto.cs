namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string AllowedGrantTypes { get; set; }
        public string AllowedScopes { get; set; }
    }
}