using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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

        //
        // GET: /Profile/Index
        public async Task<ViewResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var albums = await _albumService.GetAlbumsAsync(userId);
            return View(albums);
        }

        //
        // GET: /Profile/Album
        public async Task<ActionResult> Album(int? id)
        {
            if (id != null)
            {
                var userId = User.Identity.GetUserId();
                var album = await _albumService.GetAlbumAsync(id.Value, userId);
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
        public async Task<ActionResult> DeleteAlbum(int? id)
        {
            if (id != null)
            {
                var userId = User.Identity.GetUserId();
                var album = await _albumService.GetAlbumAsync(id.Value, userId);

                if (album != null)
                {
                    await _albumService.DeleteAlbumAsync(id.Value);
                }
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Profile/Setting
        public ActionResult AddPhoto(int? albumId)
        {
            if(albumId == null)
            {
                return RedirectToAction("Index");
            }

            return View(new PhotoViewModel { CurrentAlbumId = albumId.Value });
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
                var picture = new Photo
                {
                    ImageMimeType = photoModel.Image.ContentType,
                    Image = new byte[photoModel.Image.ContentLength],
                    AlbumId = photoModel.CurrentAlbumId,
                };

                photoModel.Image.InputStream.Read(picture.Image, 0, photoModel.Image.ContentLength);
                await _photoService.AddPhotoAsync(picture);
                // photoModel.Image.SaveAs(Server.MapPath("~/Image/" + System.IO.Path.GetFileName(photoModel.Image.FileName)));
                return RedirectToAction("Album", new { id = photoModel.CurrentAlbumId });
            }

            return View(photoModel);
        }

        //
        // GET: Profile/DeletePhoto
        [AllowAnonymous]
        public async Task<ActionResult> DeletePhoto(int? id)
        {
            if(id != null)
            {
                var userId = User.Identity.GetUserId();
                var photo = await _photoService.GetPhotoAsync(id.Value);
                var album = await _albumService.GetAlbumAsync(photo.AlbumId, userId);

                if (album != null)
                {
                    await _photoService.DeletePhotoAsync(id.Value);
                    return RedirectToAction("Album", new { id = album.AlbumId });
                }
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<FileContentResult> GetPhoto(int? photoId)
        {
            if(photoId != null)
            {
                var userId = User.Identity.GetUserId();
                var photo = await _photoService.GetPhotoAsync(photoId.Value);

                if (photo == null)
                {
                    return null;
                }

                var album = await _albumService.GetAlbumAsync(photo.AlbumId, userId);

                if (album != null)
                {
                    return File(photo.Image, photo.ImageMimeType);
                }
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