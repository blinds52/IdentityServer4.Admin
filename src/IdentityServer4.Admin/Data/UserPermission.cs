using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Data
{
    public class UserPermission : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Permission { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}