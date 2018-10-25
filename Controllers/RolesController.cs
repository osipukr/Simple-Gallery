using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Simply_Gallery.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

         // GET: Roles
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await RoleManager.CreateAsync(new ApplicationRole
                {
                    Name = model.Name,
                    Description = model.Description
                });

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    ModelState.AddModelError("", "Что-то пошло не так");
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return View(new RoleModel { Id = role.Id, Name = role.Name, Description = role.Description });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Edit(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(model.Id);
                if (role != null)
                {
                    role.Name = model.Name;
                    role.Description = model.Description;

                    var result = await RoleManager.UpdateAsync(role);

                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError("", "Что-то пошло не так");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
    }
}