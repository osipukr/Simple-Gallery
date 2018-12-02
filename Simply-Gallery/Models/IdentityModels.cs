using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Simply_Gallery.Models.Gallery;
using System.Data.Entity;
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
            : base("DefaultDBConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }

        public static ApplicationContext Create() => new ApplicationContext();
    }
}