using Microsoft.AspNet.Identity.Owin;
using Simply_Gallery.App_Start;
using Simply_Gallery.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;
using System;
using Simply_Gallery.Models.Gallery;
using Simply_Gallery.Services;
using Simply_Gallery.ViewModels;

namespace Simply_Gallery.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IPhotoService _pictureService;
        private readonly IAlbumService _albumService;

        public ProfileController(IPhotoService pictureService, IAlbumService albumService)
        {
            _pictureService = pictureService;
            _albumService = albumService;
        }

        //
        // GET: /Profile/Index
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var albums = await _albumService.GetAlbumsAsync(userId);
            return View(albums);
        }

        //
        // GET: /Profile/CreateAlbum
        public ActionResult CreateAlbum()
        {
            return View();
        }

        //
        // POST: /Profile/CreateAlbum
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateAlbum(AlbumViewModel albumModel)
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
                    return RedirectToAction("Album", new { id = album.AlbumId });
                }

                ModelState.AddModelError("", "Альбом с таким именем уже создан");
            }

            return View(albumModel);
        }

        //
        // GET: /Profile/Album
        public async Task<ActionResult> Album(int? id)
        {
            if (id != null)
            {
                var userId = User.Identity.GetUserId();
                var album = await _albumService.GetAlbumAsync(Convert.ToInt32(id), userId);

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

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPhoto(PhotoModel photoModel, HttpPostedFileBase photo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var picture = new Picture
        //        {
        //            ImageMimeType = photo.ContentType,
        //            Image = new byte[photo.ContentLength],
        //            AlbumId = photoModel.CurrentAlbumId
        //        };

        //        photo.InputStream.Read(picture.Image, 0, photo.ContentLength);

        //        db.Pictures.Add(picture);
        //        await db.SaveChangesAsync();
        //    }

        //    return PartialView("ProfileContent/Photos");
        //}

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