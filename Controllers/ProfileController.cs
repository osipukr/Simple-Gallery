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
        #endregion


        //
        // GET: /Profile/Index
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            ViewBag.Roles = user.Roles.Select(x => RoleManager.Roles.FirstOrDefault(r => r.Id == x.RoleId).Description);
            var UserAlbum = db.Albums.Where(x => x.UserId == userId).ToList();
            return View(UserAlbum);
        }

        //
        // GET: /Profile/CreateAlbum
        public ActionResult CreateAlbum()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateAlbum(AlbumModel albumModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var result = db.Albums.FirstOrDefault(x => x.Name == albumModel.Name && x.UserId == userId);
                if (result == null)
                {
                    var album = new Album
                    {
                        Name = albumModel.Name,
                        UserId = userId
                    };
                    db.Albums.Add(album);
                    db.SaveChanges();
                    return RedirectToAction("Album", new { id = Convert.ToString(album.AlbumId) });
                }
                ModelState.AddModelError("", "Альбом с таким названием уже создан");
            }
            return View();
        }

        //
        // GET: /Profile/Album
        public ActionResult Album(string id)
        {
            int? albumId = Convert.ToInt32(id);
            if (albumId != null)
            {
                var userId = User.Identity.GetUserId();
                var album = db.Albums.FirstOrDefault(a => a.AlbumId == albumId && a.UserId == userId);

                if (album != null)
                {
                    return View(album);
                }
            }

            return RedirectToAction("Index");
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

        //[AllowAnonymous]
        //public FileContentResult GetUserPhoto()
        //{
        //    var profile = db.Profiles.FirstOrDefault(p => p.UserId == User.Identity.GetUserId());
        //    if(profile != null)
        //    {
        //        if (profile.UserImage != null)
        //        {
        //            return File(profile.UserImage, profile.ImageMimeType);
        //        }
        //    }

        //    return null;
        //}
    }
}