using System;
using System.Collections.Generic;
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

        // менеджер пользователей
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        //
        // GET: /Profile/Index
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var albums = await _albumService.GetAlbumsAsync(User.Identity.GetUserId());
            return View(albums);
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            return  PartialView("Shared/_Header", UserManager.FindById(User.Identity.GetUserId()));
        }

        //
        // GET: /Profile/Album
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
        public ActionResult Setting()
        {
            return View();
        }
    }
}