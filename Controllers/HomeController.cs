using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Simply_Gallery.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Simply_Gallery.App_Start;

namespace Simply_Gallery.Controllers
{
    public class HomeController : Controller
    {
        #region Manager
        // менеджер пользователей
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        // аунтификационный менеджер
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        #endregion

        //
        // GET: /Home/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Home/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            // валидация
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // добавляем пользователю роль
                    await UserManager.AddToRoleAsync(user.Id, "user");
                    // входим в аккаунт
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return JavaScript("location.reload()");

                    //await UserManager.AddToRoleAsync(user.Id, "user");
                    //// удаляются ранние куки
                    //AuthenticationManager.SignOut();
                    //// устанавливаем новые аутентификационные куки
                    //AuthenticationManager.SignIn(new AuthenticationProperties
                    //{
                    //    IsPersistent = true
                    //}, await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie));
                    //return JavaScript("location.reload()");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return PartialView("_RegisterPartial");
        }

        //
        // POST: /Home/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            // валидация
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return JavaScript("location.reload()");
                    default:
                        ModelState.AddModelError("", "Неверный логин или пароль"); break;
                }

                /*
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", (await UserManager.FindByNameAsync(model.UserName) == null) ?
                        "Неверное имя пользователя" : "Неверный пароль");
                }
                else
                {
                    // создание настроек аутентификационного тикета
                    var claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    // удаляются ранние куки
                    AuthenticationManager.SignOut();
                    // устанавливаем новые аутентификационные куки
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    return JavaScript("location.reload()");
                }
                */
            }
            return PartialView("_LoginPartial");
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}