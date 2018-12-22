using Microsoft.AspNet.Identity.EntityFramework;
using Simply_Gallery.DAL.EF;
using Simply_Gallery.DAL.Entities;
using Simply_Gallery.DAL.Identity;
using Simply_Gallery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simply_Gallery.DAL.Repositories
{
    public class IdentityUnitOfWork : IUnitOfWork
    {
        private ApplicationContext db;

        public ApplicationUserManager UserManager { get; private set; }

        public ApplicationRoleManager RoleManager { get; private set; }

        public ApplicationSignInManager SignInManager { get; private set; }

        public IClientManager ClientManager { get; private set; }

        public IdentityUnitOfWork(string connectionString)
        {
            db = new ApplicationContext(connectionString);

            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            RoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            ClientManager = new ClientManager(db);
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UserManager.Dispose();
                    RoleManager.Dispose();
                    ClientManager.Dispose();
                }

                disposed = true;
            }
        }
    }
}