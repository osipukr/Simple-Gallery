using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Simply_Gallery.DAL.EF;
using Simply_Gallery.DAL.Entities;

namespace Simply_Gallery.DAL.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
                                                IOwinContext context)
        {
            // получаем контекст бд
            var db = context.Get<ApplicationContext>();

            // создаем менеджера для данного контекста
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));

            // настройка валидации для пользователя
            manager.UserValidator = new ApplicationUserValidator(manager);

            // настройка валидации для пароля
            manager.PasswordValidator = new PasswordValidator
            {
                // Необходимая длина пароля
                RequiredLength = 6
            };

            return manager;
        }
    }
}