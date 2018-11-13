using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Simply_Gallery.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Simply_Gallery.App_Start;
using Simply_Gallery.ViewModels;

namespace Simply_Gallery.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        // менеджер входа
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // менеджер пользователей
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // валидация
            if (ModelState.IsValid)
            {
                if(model.UserName.ToLower().Contains("admin") || model.UserName.ToLower().Contains("админ"))
                {
                    ModelState.AddModelError("", "Имя пользователя не должно содердать слова 'admin'");
                    return PartialView("_RegisterPartial", model);
                }

                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // добавляем пользователю роль
                   // await UserManager.AddToRoleAsync(user.Id, "user");

                    // входим в аккаунт
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return JavaScript("location.reload()");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            return PartialView("Shared/_Register", model);
        }

        //
        // POST: /Home/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            // валидация
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, shouldLockout: false);
                
                switch (result)
                {
                    case SignInStatus.Success : return JavaScript("location.reload()");
                    default:
                        ModelState.AddModelError("", "Неверное имя пользователя или пароль");
                        break;
                }
            }

            return PartialView("Shared/_Login", model);
        }

        //
        // POST: /Home/LogOut
        [HttpPost]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion
    }
}