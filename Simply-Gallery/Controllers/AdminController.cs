using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using Simply_Gallery.ViewModels;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Simply_Gallery.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
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

        // менеджер ролей
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Admin/Index
        public async Task<ActionResult> Index() => View(await RoleManager.Roles.ToListAsync());

        //
        // GET: /Admin/Create
        public ActionResult Create() => View();

        //
        // POST: /Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await RoleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("", "Роль '" + model.Name + "' уже существует");
                return View(model);
            }

            var result = await RoleManager.CreateAsync(new ApplicationRole
            {
                Name = model.Name,
                Description = model.Description
            });

            if (result.Succeeded)
            {
                TempData["Message"] = "Новая роль успешно добавлена";
                return RedirectToAction("Index");
            }

            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Admin/Edit
        public async Task<ActionResult> Edit(string roleId)
        {
            if (roleId == null)
            {
                return RedirectToAction("Index");
            }

            var role = await RoleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                return View(new EditRoleViewModel
                {
                    OldName = role.Name,
                    NewName = role.Name,
                    OldDescription = role.Description,
                    NewDescription = role.Description
                });
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Admin/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await RoleManager.FindByNameAsync(model.OldName);

            if (role != null)
            {
                if(model.OldName == model.NewName &&
                    model.OldDescription == model.NewDescription)
                {
                    return RedirectToAction("Index");
                }

                role.Name = model.NewName;
                role.Description = model.NewDescription;

                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Роль успешно изменена";
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            return View(model);
        }

        //
        // GET: /Admin/Delete
        public async Task<ActionResult> Delete(string roleId)
        {
            if (roleId == null)
            {
                return RedirectToAction("Index");
            }

            var role = await RoleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Роль успешно удалена";
                }
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Admin/Users
        public async Task<ActionResult> Users(string roleId)
        {
            if (roleId == null)
            {
                return RedirectToAction("Index");
            }

            var role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RedirectToAction("Index");
            }

            var roleModel = new UsersRoleViewModel { Name = role.Name };
            foreach (var userRole in role.Users)
            {
                roleModel.Users.Add(await UserManager.FindByIdAsync(userRole.UserId));
            }

            return View(roleModel);
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

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helper

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion
    }
}