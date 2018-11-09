using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;
using System;

namespace Simply_Gallery.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        #region Helpers
        // менеджер пользователей
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // менеджер ролей
        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        // контекст бд
        private ApplicationContext db = new ApplicationContext();

        //// текущий авторизованный пользователь
        //private ApplicationUser CurrentUser { get; set; }

        //// профиль пользователя
        //private Profile CurrentProfile { get; set; }
        #endregion

        //private async Task GetCurrent()
        //{
        //    CurrentUser = await UserManager.FindByNameAsync(User.Identity.Name);
        //    CurrentProfile = db.Profiles.FirstOrDefault(p => p.UserId == CurrentUser.Id);
        //}

        //
        // GET: /Profile/Index
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.Roles = user.Roles.Select(x => RoleManager.Roles.FirstOrDefault(r => r.Id == x.RoleId).Description);
            return View(db.Profiles.FirstOrDefault(u => u.UserId == user.Id));
        }

        //
        // GET: /Profile/Album
        public ActionResult Album(int? id)
        {
            if (id != null)
            {
                var album = db.Albums.FirstOrDefault(a => a.AlbumId == id && a.Profile.UserId == User.Identity.GetUserId());
                if (album != null)
                {
                    return View(album);
                }
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Profile/CreateAlbum
        public ActionResult CreateAlbum()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateAlbum(AlbumModel albumModel)
        {
            if(ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var profileId = db.Profiles.FirstOrDefault(x => x.UserId == userId).Id;
                var result = db.Albums.FirstOrDefault(a => a.Name == albumModel.Name && a.Profile.UserId == userId);
                if(result == null)
                {
                    var album = new Album
                    {
                        Name = albumModel.Name,
                        ProfileId = profileId,
                    };

                    db.Albums.Add(album);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Album", new { id = album.AlbumId });
                }
                ModelState.AddModelError("", "Альбом с таким названием уже создан");
            }
            return View();
        }

        //
        // GET: /Profile/Setting
        public ActionResult Setting()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoto(PhotoModel photoModel, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                var picture = new Picture
                {
                    ImageMimeType = photo.ContentType,
                    Image = new byte[photo.ContentLength],
                    AlbumId = photoModel.CurrentAlbumId
                };

                photo.InputStream.Read(picture.Image, 0, photo.ContentLength);

                db.Pictures.Add(picture);
                await db.SaveChangesAsync();
            }

            return PartialView("ProfileContent/Photos");
        }

        //[AllowAnonymous]
        //public FileContentResult GetPhotoFromAlbum(int albumId, int photoId)
        //{
        //    var album = CurrentProfile.Albums.FirstOrDefault(a => a.AlbumId == albumId);
        //    if (album != null)
        //    {
        //        var photo = album.Pictures.FirstOrDefault(p => p.PictureId == photoId);
        //        if(photo != null)
        //        {
        //            return File(photo.Image, photo.ImageMimeType);
        //        }
        //    }

        //    return null;
        //}

        [AllowAnonymous]
        public FileContentResult GetUserPhoto()
        {
            var profile = db.Profiles.FirstOrDefault(p => p.UserId == User.Identity.GetUserId());
            if(profile != null)
            {
                if (profile.UserImage != null)
                {
                    return File(profile.UserImage, profile.ImageMimeType);
                }
            }

            return null;
        }
    }
}