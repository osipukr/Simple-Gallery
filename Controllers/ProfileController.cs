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
        public async Task<ViewResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var albums = await _albumService.GetAlbumsAsync(userId);
            return View(albums);
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);

            return PartialView("Shared/_Header", user);
        }

        //
        // GET: /Profile/Album
        public async Task<ActionResult> Album(int? albumId)
        {
            if (albumId != null)
            {
                var userId = User.Identity.GetUserId();
                var album = await _albumService.GetAlbumAsync(albumId.Value, userId);
                //album.Photos = await _photoService.GetPhotosAsync(album.AlbumId);

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
        [AllowAnonymous]
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
                    return RedirectToAction("Album", new { id = album.AlbumId });
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
                var userId = User.Identity.GetUserId();
                var album = await _albumService.GetAlbumAsync(albumId.Value, userId);

                if (album != null)
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
                var album = await _albumService.GetAlbumAsync(albumId.Value, User.Identity.GetUserId());

                if (album != null)
                {
                    return View(new PhotoViewModel { CurrentAlbumId = albumId.Value });
                }
                
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Profile/AddPhoto
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoto(PhotoViewModel photoModel)
        {
            if (ModelState.IsValid)
            {
                var photo = new Photo
                {
                    ImageMimeType = photoModel.Image.ContentType,
                    Image = new byte[photoModel.Image.ContentLength],
                    AlbumId = photoModel.CurrentAlbumId,
                };

                photoModel.Image.InputStream.Read(photo.Image, 0, photoModel.Image.ContentLength);
                await _photoService.AddPhotoAsync(photo);

                return RedirectToAction("Album", new { albumId = photoModel.CurrentAlbumId });
            }

            return View(photoModel);
        }

        //
        // GET: Profile/DeletePhoto
        [AllowAnonymous]
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
                        return RedirectToAction("Album", new { albumId = album.AlbumId });
                    }
                }
            }
                
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<FileContentResult> GetPhoto(int? photoId)
        {
            if(photoId != null)
            {
                var photo = await _photoService.GetPhotoAsync(photoId.Value);

                if (photo == null)
                {
                    return null;
                }

                var album = await _albumService.GetAlbumAsync(photo.AlbumId, User.Identity.GetUserId());

                if (album != null)
                {
                    return File(photo.Image, photo.ImageMimeType);
                }
            }

            return null;
        }

        [ChildActionOnly]
        public async Task<FileResult> GetUserPhoto(string userId)
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
        public ActionResult Setting()
        {
            return View();
        }
    }
}