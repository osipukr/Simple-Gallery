using Microsoft.AspNet.Identity.EntityFramework;

namespace Simply_Gallery.DAL.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
    }
}