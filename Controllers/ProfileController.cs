using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models.Gallery;
using Simply_Gallery.Services;
using Simply_Gallery.ViewModels;

namespace Simply_Gallery.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IAlbumService _albumService;

        public ProfileController()
        {
        }

        public ProfileController(IPhotoService photoService, IAlbumService albumService)
        {
            _photoService = photoService;
            _albumService = albumService;
        }

        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        private ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().GetUserManager<ApplicationSignInManager>();

        //
        // GET: /Profile/Index
        public async Task<ActionResult> Index()
        {
            var albums = await _albumService.GetAlbumsAsync(User.Identity.GetUserId());
            return View(albums);
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var currentProfile = new ProfileViewModel
            {
                UserId = User.Identity.GetUserId(),
                UserName = User.Identity.GetUserName(),
                UserRoles = UserManager.GetRoles(User.Identity.GetUserId()),
                UserImage = UserManager.FindById(User.Identity.GetUserId()).Image
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
        public ViewResult AddAlbum()
        {
            return View();
        }

        //
        // POST: /Profile/AddAlbum
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAlbum(AlbumViewModel albumModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var result = await _albumService.GetAlbumAsync(albumModel.Name, userId);

                if (result == null)
                {
                    var album = new Album
                    {
                        Name = albumModel.Name,
                        UserId = userId
                     };

                    await _albumService.AddAlbumAsync(album);
                    return RedirectToAction("Album", new { albumId = album.Id });
                }

                ModelState.AddModelError("", "Альбом с таким именем уже создан");
            }

            return View(albumModel);
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

            return RedirectToAction("Index");
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
            if (!ModelState.IsValid)
            {
                return View(photoModel);
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            if (!photoModel.Image.ContentType.Contains("image") &&
                !formats.Any(item => photoModel.Image.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "Этот файл не может быть загружен в галерею");

                return View(photoModel);
            }
            
            if(photoModel.Image.ContentLength > Math.Pow(5, Math.Pow(10, 6)))
            {
                ModelState.AddModelError("", "Привышен размер загружаемого файла");

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
        [ChildActionOnly]
        public async Task<ActionResult> GetUserPhoto(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if(user != null)
            {
                return File(user.Image, user.ImageMimeType);
            }

            return null;
        }

        //
        // GET: /Profile/Setting
        public ActionResult Setting(ProfileMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ProfileMessageId.AddPhoneSuccess ? "Номер телефона успешно добавлен."
                : message == ProfileMessageId.RemovePhoneSuccess ? "Номер телефона был удалён."
                : message == ProfileMessageId.ChangePasswordSuccess ? "Пароль успешно изменен."
                : message == ProfileMessageId.ChangeEmailSuccess ? "Почта успешно изменена."
                : message == ProfileMessageId.ChangeNameSuccess ? "Имя успешно изменено."
                : message == ProfileMessageId.Error ? "Произошла ошибка"
                : "";

            return View();
        }

        //
        // GET: /Profile/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return RedirectToAction("Setting", new { Message = ProfileMessageId.ChangePasswordSuccess });
            }

            AddErrors(result);

            return View(model);
        }

        //
        // GET: /Profile/ChangeName
        public ActionResult ChangeName()
        {
            return View(new ChangeNameViewModel { CurrentUserName = User.Identity.GetUserName() });
        }

        //
        // POST: /Profile/ChangeName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeName(ChangeNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewUserName.ToLower().Contains("admin") || model.NewUserName.ToLower().Contains("админ"))
            {
                ModelState.AddModelError("", "Имя пользователя не должно содердать слова 'admin'");
                return View(model);
            }

            if(await UserManager.FindByNameAsync(model.NewUserName) != null)
            {
                ModelState.AddModelError("", "Пользователя с таким именем уже зарегистрирован");
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                user.UserName = model.NewUserName;
                await UserManager.UpdateAsync(user);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                return RedirectToAction("Setting", new { Message = ProfileMessageId.ChangeNameSuccess });
            }

            ModelState.AddModelError("", "Произошла ошибка при изменении имени, попробуйте обновите страницу");

            return View(model);
        }

        //
        // GET: /Profile/ChangeEmail
        public async Task<ActionResult> ChangeEmail()
        {
            return View(new ChangeEmailViewModel { CurrentEmail = await UserManager.GetEmailAsync(User.Identity.GetUserId()) });
        }

        //
        // POST: /Profile/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            if(await UserManager.FindByEmailAsync(model.NewEmail) != null)
            {
                ModelState.AddModelError("", "Пользователь с таким e-mail уже зарегистрирован");
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if(user != null)
            {
                user.Email = model.NewEmail;
                await UserManager.UpdateAsync(user);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                return RedirectToAction("Setting", new { Message = ProfileMessageId.ChangeEmailSuccess });
            }

            ModelState.AddModelError("", "Произошла ошибка при изменении e-mail, попробуйте обновите страницу");

            return View(model);
        }
 
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ProfileMessageId
        {
            AddPhoneSuccess,
            RemovePhoneSuccess,
            ChangePasswordSuccess,
            ChangeEmailSuccess,
            ChangeNameSuccess,
            Error
        }

        #endregion
    }
}