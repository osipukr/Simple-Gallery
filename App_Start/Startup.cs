using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Simply_Gallery.Models;

[assembly: OwinStartup(typeof(Simply_Gallery.App_Start.Startup))]

namespace Simply_Gallery.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // регистрация контекста данных
            app.CreatePerOwinContext(ApplicationContext.Create);
            // регистрация менеджера пользователей
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            // регистрация менеджера ролей
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            // использование куки для аутентификации и авторизации
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}