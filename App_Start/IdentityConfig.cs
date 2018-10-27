using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Simply_Gallery.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Simply_Gallery.App_Start
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store) { }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
                                                IOwinContext context)
        {
            // получаем контекст бд
            var db = context.Get<ApplicationContext>();
            // создаем менеджера для данного контекста
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));

            // настройка валидации для пользователя
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                // UserName должен содержать только алфавитно-цифровые символы
                AllowOnlyAlphanumericUserNames = true,
                // Email должен быть уникальным
                RequireUniqueEmail = true
            };

            // настройка валидации для пароля
            manager.PasswordValidator = new PasswordValidator
            {
                // Необходимая длина пароля
                RequiredLength = 6
            };

            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(RoleStore<ApplicationRole> store) : base(store) { }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
                                                IOwinContext context)
        {
            return new ApplicationRoleManager(new
                    RoleStore<ApplicationRole>(context.Get<ApplicationContext>()));
        }
    }
}