using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.Controllers.API
{
    public class CreateClientDto
    {
        /// <summary>
        /// Unique ID of the client
        /// </summary>
        [StringLength(200)]
        public string ClientId { get; set; }
        
        [StringLength(200)]
        public string ProtocolType{ get; set; }
    }
}