using Microsoft.AspNet.Identity.EntityFramework;
using Simply_Gallery.DAL.Entities;
using System.Data.Entity;

namespace Simply_Gallery.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(string conectionString) 
            : base(nameOrConnectionString: conectionString, throwIfV1Schema: false)
        {
        }

        public DbSet<ClientProfile> ClientProfiles { get; set; }
    }
}