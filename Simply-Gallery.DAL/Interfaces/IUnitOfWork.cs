using Simply_Gallery.DAL.Identity;
using System;
using System.Threading.Tasks;

namespace Simply_Gallery.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }

        ApplicationRoleManager RoleManager { get; }

        ApplicationSignInManager SignInManager { get; }

        IClientManager ClientManager { get; }

        Task SaveAsync();
    }
}