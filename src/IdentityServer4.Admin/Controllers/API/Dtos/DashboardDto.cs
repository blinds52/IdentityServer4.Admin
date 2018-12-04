namespace IdentityServer4.Admin.Controllers.API.Dtos
{
    public class DashboardDto
    {
        public int UserCount { get; set; }
        public int LockedUserCount { get; set; }
        public int ClientCount{ get; set; }
        public int ApiResourceCount { get; set; }
    }
}