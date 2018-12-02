using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models.Gallery;
using Simply_Gallery.ViewModels;

namespace Simply_Gallery.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IAlbumService _albumService;

        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationRoleManager _roleManager;

        public ProfileController()
        {
        }

        public ProfileController(IPhotoService photoService, IAlbumService albumService)
        {
            _photoService = photoService;
            _albumService = albumService;
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
        // GET: /Profile/Index
        public async Task<ActionResult> Index() => View(await _albumService.GetAlbumsAsync(User.Identity.GetUserId()));

        [ChildActionOnly]
        public ActionResult Header()
        {
            var userId = User.Identity.GetUserId();

            var currentProfile = new ProfileViewModel
            {
                UserId = userId,
                UserName = User.Identity.GetUserName(),
                UserRoles = RoleManager.Roles.Where(x => x.Users.Any(u => u.UserId == userId)).Select(x => x.Description).ToList(),
                UserIsImage = UserManager.FindById(userId).Image != null,
                CountAlbum = Task.Run(async () => await _albumService.GetAlbumsAsync(userId)).Result.Count()
            };

            return  PartialView("Shared/_Header", currentProfile);
        }

        //
        // GET: /Profile/Album
        public async Task<ActionResult> Album(int? albumId)
        {
            if (albumId != null)
            {
                var album = await _albumService.GetAlbumAsync(albumId.Value, User.Identity.GetUserId());

                if (album != null)
                {
                    return View(album);
                }
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Profile/AddAlbum
        public ViewResult AddAlbum() => View();

        //
        // POST: /Profile/AddAlbum
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAlbum(CreateAlbumViewModel albumModel)
        {
            if (!ModelState.IsValid)
            {
                return View(albumModel);
            }

            var userId = User.Identity.GetUserId();
            var result = await _albumService.GetAlbumAsync(albumModel.Name, userId);

            if (result == null)
            {
                var album = new Album
                {
                    Name = albumModel.Name,
                    Description = albumModel.Description,
                    UserId = userId,
                    Date = DateTime.Now
                };

                await _albumService.AddAlbumAsync(album);
                return RedirectToAction("Album", new { albumId = album.Id });
            }

            ModelState.AddModelError("", "Альбом с таким названием уже создан");
            return View(albumModel);
        }

        //
        // GET: /Profile/EditAlbum
        public async Task<ActionResult> EditAlbum(int? albumId)
        {
            if (albumId != null)
            {
                var result = await _albumService.GetAlbumAsync(albumId.Value, User.Identity.GetUserId());

                if (result != null)
                {
                    return View(new EditAlbumViewModel
                    {
                        OldName = result.Name,
                        NewName = result.Name,
                        OldDescription = result.Description,
                        NewDescription = result.Description
                    });
                }
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Profile/AddAlbum
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAlbum(EditAlbumViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.OldName == model.NewName &&
                    model.OldDescription == model.NewDescription)
            {
                return RedirectToAction("Index");
            }

            var album = await _albumService.GetAlbumAsync(model.OldName, User.Identity.GetUserId());

            if (album == null)
            {
                ModelState.AddModelError("", string.Format("Альбома '{0}' уже не существует", model.OldName));
                return View(model);
            }

            var result = await _albumService.GetAlbumAsync(model.NewName, User.Identity.GetUserId());

            if(result != null && model.OldName != model.NewName)
            {
                ModelState.AddModelError("", "Альбом с таким названием уже существует");
                return View(model);
            }

            album.Name = model.NewName;
            album.Description = model.NewDescription;

            await _albumService.UpdateAlbumAsync(album);
            TempData["Message"] = "Альбом успешно изменён";
            return RedirectToAction("Index");
        }

        //
        // GET: /Profile/DeleteAlbum
        public async Task<ActionResult> DeleteAlbum(int? albumId)
        {
            if (albumId != null)
            {
                var result = await _albumService.GetAlbumAsync(albumId.Value, User.Identity.GetUserId());

                if (result != null)
                {
                    await _albumService.DeleteAlbumAsync(albumId.Value);
                }
            }

            TempData["Message"] = "Альбом успешно удалён";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> SearchAlbum()
        {
            var requests = Request.Params["Search"];
            var albums = await _albumService.GetAlbumsAsync(User.Identity.GetUserId());

            if(!string.IsNullOrEmpty(requests))
            {
                return PartialView("Shared/_Albums", albums.Where(a => a.Name.ToLower().Contains(requests) || a.Description.ToLower().Contains(requests)));
            }

            return PartialView("Shared/_Albums", albums);
        }

        //
        // GET: /Profile/Setting
        public async Task<ActionResult> AddPhoto(int? albumId)
        {
            if(albumId != null)
            {
                var result = await _albumService.GetAlbumAsync(albumId.Value, User.Identity.GetUserId());

                if (result != null)
                {
                    return View(new PhotoViewModel { CurrentAlbumId = albumId.Value });
                }
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Profile/AddPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoto(PhotoViewModel photoModel)
        {
            if (!ModelState.IsValid || !isValidImage(photoModel.Image))
            {
                return View(photoModel);
            }

            var photo = new Photo
            {
                ImageMimeType = photoModel.Image.ContentType,
                Image = new byte[photoModel.Image.ContentLength],
                AlbumId = photoModel.CurrentAlbumId,
            };

            await photoModel.Image.InputStream.ReadAsync(photo.Image, 0, photoModel.Image.ContentLength);
            await _photoService.AddPhotoAsync(photo);

            return RedirectToAction("Album", new { albumId = photoModel.CurrentAlbumId });
        }

        //
        // GET: Profile/DeletePhoto
        public async Task<ActionResult> DeletePhoto(int? photoId)
        {
            if(photoId != null)
            {
                var photo = await _photoService.GetPhotoAsync(photoId.Value);

                if (photo != null)
                {
                    var album = await _albumService.GetAlbumAsync(photo.AlbumId, User.Identity.GetUserId());

                    if (album != null)
                    {
                        await _photoService.DeletePhotoAsync(photoId.Value);
                        return RedirectToAction("Album", new { albumId = album.Id });
                    }
                }
            }
                
            return RedirectToAction("Index");
        }

        //
        // GET: Profile/GetPhoto
        public async Task<ActionResult> GetPhoto(int? photoId)
        {
            if(photoId != null)
            {
                var photo = await _photoService.GetPhotoAsync(photoId.Value);

                if (photo != null)
                {
                    var album = await _albumService.GetAlbumAsync(photo.AlbumId, User.Identity.GetUserId());

                    if (album != null)
                    {
                        return File(photo.Image, photo.ImageMimeType);
                    }
                }
            }

            Response.StatusCode = 404;
            return View("Error");
        }

        //
        // GET: /Profile/GetUserPhoto
        public async Task<ActionResult> GetUserPhoto()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if(user != null)
            {
                return File(user.Image, user.ImageMimeType);
            }

            return null;
        }

        //
        // GET: /Profile/Setting
        public async Task<ActionResult> Setting(string act)
        {
            return
                act == "changeName" ? PartialView("Setting/_ChangeName", new ChangeNameViewModel { CurrentUserName = User.Identity.GetUserName() })
                : act == "changeEmail" ? PartialView("Setting/_ChangeEmail", new ChangeEmailViewModel { CurrentEmail = await UserManager.GetEmailAsync(User.Identity.GetUserId()) })
                : act == "changePassword" ? PartialView("Setting/_ChangePassword")
                : act == "changeAvatar" ? PartialView("Setting/_ChangeAvatar")
                : PartialView("Setting/_Index");
        }

        //
        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Setting/_ChangePassword", model);
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                TempData["Message"] = "Пароль успешно изменен";
                return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting")));
            }

            AddErrors(result);
            return PartialView("Setting/_ChangePassword", model);
        }

        //
        // POST: /Profile/ChangeName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeName(ChangeNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Setting/_ChangeName", model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                user.UserName = model.NewUserName;
                var result = await UserManager.UpdateAsync(user);
                
                if(result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    TempData["Message"] = "Имя пользователя успешно изменено";
                    return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting")));
                }

                AddErrors(result);
            }

            return PartialView("Setting/_ChangeName", model);
        }

        //
        // POST: /Profile/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return PartialView("Setting/_ChangeEmail", model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if(user != null)
            {
                user.Email = model.NewEmail;
                var result = await UserManager.UpdateAsync(user);
                
                if(result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    TempData["Message"] = "Почта успешно изменена";
                    return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting")));
                }

                AddErrors(result);
            }

            return PartialView("Setting/_ChangeEmail", model);
        }

        //
        // POST: /Profile/ChangeAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeAvatar()
        {
            // получаем изображения
            var image = Request.Files["image"];

            if (!ModelState.IsValid || !isValidImage(image))
            {
                return PartialView("Setting/_ChangeAvatar");
            }

            // получаем пользователя
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                user.Image = new byte[image.ContentLength];
                user.ImageMimeType = image.ContentType;

                await image.InputStream.ReadAsync(user.Image, 0, image.ContentLength);

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    TempData["Message"] = "Аватар пользователя успешно изменён";
                    return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting")));
                }

                AddErrors(result);
            }

            return PartialView("Setting/_ChangeAvatar");
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

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool isValidImage(HttpPostedFileBase image)
        {
            if (image == null)
            {
                ModelState.AddModelError("", "Выберите изображение");
                return false;
            }

            // максимальный размер файла 2 Мб
            int maxSizeFile = 2 * 1024 * 1024;

            // допустимые MIME-типы для файлов
            var mimes = new string[]
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            // проверки на допустимый размер файла
            if (image.ContentLength > maxSizeFile)
            {
                ModelState.AddModelError("", "Размер изображения больше 2мб");
                return false;
            }

            // проверки на допустимый формат файла
            if (mimes.FirstOrDefault(x => x == image.ContentType) == null)
            {
                ModelState.AddModelError("", "Не допустимый формат изображения");
                return false;
            }

            return true;
        }

        #endregion
    }
}