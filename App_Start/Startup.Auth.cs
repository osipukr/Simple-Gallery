using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using System;

namespace Simply_Gallery
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // регистрация контекста данных
            app.CreatePerOwinContext(ApplicationContext.Create);
            // регистрация диспечера пользователей
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            // регистрация диспечера ролей
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            // регистрация диспечера входа
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            
            // использование куки для аутентификации и авторизации
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Index"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}