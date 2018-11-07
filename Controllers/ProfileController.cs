using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Simply_Gallery.Controllers
{
    public class ProfileController : Controller
    {
        #region Manager
        // менеджер пользователей
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        #endregion

        // GET: Profile
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var userName = User.Identity.Name;
            var user = await UserManager.FindByNameAsync(userName);
            return View(user);
        }
    }
}