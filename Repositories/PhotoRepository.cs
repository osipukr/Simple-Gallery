using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Simply_Gallery.Common;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        public PhotoRepository()
        {
        }

        public async Task<IEnumerable<Photo>> GetPhotosAsync(int albumId)
        {
            var result = new List<Photo>();

            using (var galleryContext = new GalleryContext())
            {
                result = await galleryContext.Photos.Where(x => x.AlbumId == albumId).ToListAsync();
            }

            return result;
        }

        public async Task<Photo> GetPhotoAsync(int photoId)
        {
            Photo result = null;

            using (var galleryContext = new GalleryContext())
            {
                result = await galleryContext.Photos.FirstOrDefaultAsync(x => x.PhotoId == photoId);
            }

            return result;
        }

        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            Photo result = null;

            using (var galleryContext = new GalleryContext())
            {
                result = galleryContext.Photos.Add(photo);
                await galleryContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            using (var galleryContext = new GalleryContext())
            {
                var picture = await galleryContext.Photos.FirstOrDefaultAsync(x => x.PhotoId == photoId);

                galleryContext.Entry(picture).State = EntityState.Deleted;

                await galleryContext.SaveChangesAsync();
            }
        }

        public async Task<Photo> UpdatePhotoAsync(Photo photo)
        {
            using (var galleryContext = new GalleryContext())
            {
                galleryContext.Entry(photo).State = EntityState.Modified;

                await galleryContext.SaveChangesAsync();
            }

            return photo;
        }
    }
}