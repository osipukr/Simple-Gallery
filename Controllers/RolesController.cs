using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using Simply_Gallery.ViewModels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Simply_Gallery.Controllers
{
    //[Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        //
        // GET: /Roles/Index
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(await RoleManager.RoleExistsAsync(model.Name) == false)
                {
                    var result = await RoleManager.CreateAsync(new ApplicationRole
                    {
                        Name = model.Name,
                        Description = model.Description
                    });

                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError("", "Ошибка при создании новой роли");
                }
                else
                    ModelState.AddModelError("", "Роль " + model.Name + " уже существует");
            }
            return View(model);
        }

        //
        // GET: /Roles/Edit
        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return View(new RoleViewModel { Name = role.Name, Description = role.Description });
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Roles/Edit
        [HttpPost]
        public async Task<ActionResult> Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByNameAsync(model.Name);
                if (role != null)
                {
                    role.Name = model.Name;
                    role.Description = model.Description;

                    var result = await RoleManager.UpdateAsync(role);

                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError("", "Ошибка при изменении роли");
                }
            }
            return View(model);
        }

        //
        // GET: /Roles/Delete
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded == false)
                    ModelState.AddModelError("", "Ошибка при удалении роли");
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/List
        public async Task<ActionResult> List(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                var roleModel = new UsersRoleViewModel { Name = role.Name};
                foreach(var item in role.Users)
                {
                    var user = await UserManager.FindByIdAsync(item.UserId);
                    if(user != null)
                        roleModel.Users.Add(user);
                }
                return View(roleModel);
            }
            return RedirectToAction("Index");
        }
    }
}