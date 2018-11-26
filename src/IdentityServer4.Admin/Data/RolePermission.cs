using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Data
{
    public class RolePermission : IEntity<int>
    {
        [Key] public int Id { get; set; }
        public string RoleId { get; set; }
        public string Permission { get; set; }
    }
}