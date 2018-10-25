using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Simply_Gallery.Models
{
    #region UserAndRole
    public class ApplicationUser : IdentityUser
    {

    }

    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
    }
    #endregion

    #region Context
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("DbConnection", false) { }

        public static ApplicationContext Create() => new ApplicationContext();
    }
    #endregion

    #region Managers
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
    #endregion
}