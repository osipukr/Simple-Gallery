using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace Simply_Gallery.Controllers
{
    public class ProfileController : Controller
    {
        #region Helpers
        // менеджер пользователей
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // менеджер ролей
        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();

        // контекст бд
        private ApplicationContext db = new ApplicationContext();
        
        // текущий авторизованный пользователь
        private ApplicationUser CurrentUser { get; set; }

        // профиль пользователя
        private Profile CurrentProfile { get; set; }
        #endregion

        //
        // GET: /Profile/Index
        [Authorize]
        public async Task<ActionResult> Index()
        {
            CurrentUser = await UserManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Roles = CurrentUser.Roles.Select(x => RoleManager.Roles.FirstOrDefault(r => r.Id == x.RoleId).Description);
            CurrentProfile = db.Profiles.FirstOrDefault(p => p.UserId == CurrentUser.Id);

            return View(CurrentProfile);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAlbum(AlbumModel albumModel)
        {
            if(ModelState.IsValid)
            {
                var result = CurrentProfile.Albums.FirstOrDefault(a => a.Name == albumModel.Name);
                if(result == null)
                {
                    var album = new Album
                    {
                        Name = albumModel.Name,
                        ProfileId = CurrentProfile.Id
                    };

                    db.Albums.Add(album);
                    await db.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", "Альбом с таким названием уже создан");
                    return PartialView("Shared/_CreateAlbum");
                }
            }

            return PartialView("ProfileContent/Albums");
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
            return File(CurrentProfile.UserImage, CurrentProfile.ImageMimeType);
        }
    }
}