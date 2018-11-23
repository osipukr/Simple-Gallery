using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using Simply_Gallery.ViewModels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Simply_Gallery.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        //
        // GET: /Roles/Index
        public ActionResult Index(RoleMessageId? message)
        {
            ViewBag.StatusMessage =
                message == RoleMessageId.AddRoleSuccess ? "Новая роль успешно добавлена"
                : message == RoleMessageId.RemoveRoleSuccess ? "Роль успешно удалена"
                : message == RoleMessageId.ChangeRoleSuccess ? "Роль успешно изменена"
                : message == RoleMessageId.Error ? "Произошла ошибка"
                : "";

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
                return RedirectToAction("Index", new { message = RoleMessageId.AddRoleSuccess });
            }

            ModelState.AddModelError("", "Ошибка при создании новой роли");
            return View(model);
        }

        //
        // GET: /Roles/Edit
        public async Task<ActionResult> Edit(string roleId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                return View(new EditRoleViewModel { Name = role.Name, Description = role.Description });
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await RoleManager.FindByNameAsync(model.Name);

            if (role != null)
            {
                role.Name = model.Name;
                role.Description = model.Description;

                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { message = RoleMessageId.ChangeRoleSuccess });
                }

                AddErrors(result);
            }

            return View(model);
        }

        //
        // GET: /Roles/Delete
        public async Task<ActionResult> Delete(string roleId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    AddErrors(result);
                }
            }
            return RedirectToAction("Index", new { message = RoleMessageId.RemoveRoleSuccess });
        }

        //
        // GET: /Roles/Users
        public async Task<ActionResult> Users(string roleId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RedirectToAction("Index");
            }

            var roleModel = new UsersRoleViewModel { Name = role.Name };
            foreach (var item in role.Users)
            {
                var user = await UserManager.FindByIdAsync(item.UserId);

                if (user != null)
                {
                    roleModel.Users.Add(user);
                }
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

        public enum RoleMessageId
        {
            AddRoleSuccess,
            RemoveRoleSuccess,
            ChangeRoleSuccess,
            Error
        }

        #endregion
    }
}