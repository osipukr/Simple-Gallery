using Microsoft.AspNet.Identity.EntityFramework;

namespace Simply_Gallery.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ClientProfile ClientProfile { get; set; }
    }
}