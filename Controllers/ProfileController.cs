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

        // менеджер входа
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        // менеджер пользователей
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        //
        // GET: /Profile/Index
        public async Task<ActionResult> Index(AlbumMessageId? message)
        {
            ViewBag.StatusMessage =
                message == AlbumMessageId.ChangeAlbumSuccess ? "Альбом успешно изменён"
                : message == AlbumMessageId.RemoveAlbumSuccess ? "Альбом успешно удалён"
                : message == AlbumMessageId.Error ? "Произошла ошибка"
                : "";

            return View(await _albumService.GetAlbumsAsync(User.Identity.GetUserId()));
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
                    UserId = userId
                };

                await _albumService.AddAlbumAsync(album);
                return RedirectToAction("Album", new { albumId = album.Id });
            }

            ModelState.AddModelError("", "Альбом с таким именем уже создан");
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
                        NewName = result.Name
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

            if(model.OldName != model.NewName)
            {
                var result = await _albumService.GetAlbumAsync(model.NewName, User.Identity.GetUserId());

                if(result != null)
                {
                    ModelState.AddModelError("", "Альбом с таким названием уже существуте");
                    return View(model);
                }

                var album = await _albumService.GetAlbumAsync(model.OldName, User.Identity.GetUserId());

                if(album == null)
                {
                    ModelState.AddModelError("", string.Format("Альбома '{0}' уже не существует", model.OldName));
                    return View(model);
                }

                album.Name = model.NewName;
                await _albumService.UpdateAlbumAsync(album);
            }

            return RedirectToAction("Index", new { message = AlbumMessageId.ChangeAlbumSuccess });
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

            return RedirectToAction("Index", new { message = AlbumMessageId.RemoveAlbumSuccess });
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

            if (photoModel.Image == null)
            {
                ModelState.AddModelError("", "Выберите изображение");
                return View(photoModel);
            }

            // максимальный размер файла 2 Мб
            int maxSizeFile = 2 * 1024 * 1024;

            // допустимые MIME-типы для файлов
            var mimes = new string[]
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            // проверки на допустимый размер файла
            if (photoModel.Image.ContentLength > maxSizeFile)
            {
                ModelState.AddModelError("", "Размер изображения больше 2мб");
                return View(photoModel);
            }

            // проверки на допустимый формат файла
            if (mimes.FirstOrDefault(x => x == photoModel.Image.ContentType) == null)
            {
                ModelState.AddModelError("", "Не допустимый формат изображения");
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
        public async Task<ActionResult> Setting(string act, SettingMessageId? message)
        {
            ViewBag.StatusMessage =
                message == SettingMessageId.AddPhoneSuccess ? "Номер телефона успешно добавлен"
                : message == SettingMessageId.RemovePhoneSuccess ? "Номер телефона был удалён"
                : message == SettingMessageId.ChangePasswordSuccess ? "Пароль успешно изменен"
                : message == SettingMessageId.ChangeEmailSuccess ? "Почта успешно изменена"
                : message == SettingMessageId.ChangeNameSuccess ? "Имя успешно изменено"
                : message == SettingMessageId.ChangeAvatarSuccess ? "Фотография успешно изменена"
                : message == SettingMessageId.Error ? "Произошла ошибка"
                : "";
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

                return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting", new { message = SettingMessageId.ChangePasswordSuccess })));
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

            if (model.NewUserName.ToLower().Contains("admin") || model.NewUserName.ToLower().Contains("админ"))
            {
                ModelState.AddModelError("", "Имя пользователя не должно содердать слова 'admin'");
                return PartialView("Setting/_ChangeName", model);
            }

            if(await UserManager.FindByNameAsync(model.NewUserName) != null)
            {
                ModelState.AddModelError("", "Пользователя с таким именем уже зарегистрирован");
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
                    return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting", new { message = SettingMessageId.ChangeNameSuccess })));
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

            if(await UserManager.FindByEmailAsync(model.NewEmail) != null)
            {
                ModelState.AddModelError("", "Пользователь с таким e-mail уже зарегистрирован");
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
                    return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting", new { message = SettingMessageId.ChangeEmailSuccess })));
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
            if (!ModelState.IsValid)
            {
                return PartialView("Setting/_ChangeAvatar");
            }

            // получаем изображения
            var image = Request.Files["image"];

            if(image == null)
            {
                ModelState.AddModelError("", "Выберите изображение");
                return PartialView("Setting/_ChangeAvatar");
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
                return PartialView("Setting/_ChangeAvatar");
            }

            // проверки на допустимый формат файла
            if (mimes.FirstOrDefault(x => x == image.ContentType) == null)
            {
                ModelState.AddModelError("", "Не допустимый формат изображения");
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
                    return JavaScript(string.Format("location.href='{0}'", Url.Action("Setting", new { message = SettingMessageId.ChangeAvatarSuccess })));
                }

                AddErrors(result);
            }

            return PartialView("Setting/_ChangeAvatar");
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum AlbumMessageId
        {
            ChangeAlbumSuccess,
            RemoveAlbumSuccess,
            Error
        }

        public enum SettingMessageId
        {
            AddPhoneSuccess,
            RemovePhoneSuccess,
            ChangePasswordSuccess,
            ChangeEmailSuccess,
            ChangeNameSuccess,
            ChangeAvatarSuccess,
            Error
        }

        #endregion
    }
}