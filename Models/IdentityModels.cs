using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Simply_Gallery.Models
{
    public class ApplicationUser : IdentityUser
    {
        // аватар пользователя
        public byte[] Image { get; set; }

        // разширение файла аватара
        public string ImageMimeType { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole
    {
        // описание роли
        public string Description { get; set; }
    }

    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() 
            : base("DefaultConnection", false)
        {
        }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }
}