using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        public PhotoRepository()
        {
        }

        public async Task<IQueryable<Photo>> GetPhotosAsync(int albumId)
        {
            var result = new List<Photo>();

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Photos.Where(x => x.AlbumId == albumId).ToListAsync();
            }

            return result.AsQueryable();
        }

        public async Task<Photo> GetPhotoAsync(int photoId)
        {
            Photo result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Photos.FirstOrDefaultAsync(x => x.Id == photoId);
            }

            return result;
        }

        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            Photo result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = applicationContext.Photos.Add(photo);
                await applicationContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            using (var applicationContext = new ApplicationContext())
            {
                var picture = await applicationContext.Photos.FirstOrDefaultAsync(x => x.Id == photoId);

                applicationContext.Entry(picture).State = EntityState.Deleted;

                await applicationContext.SaveChangesAsync();
            }
        }

        public async Task<Photo> UpdatePhotoAsync(Photo photo)
        {
            using (var applicationContext = new ApplicationContext())
            {
                applicationContext.Entry(photo).State = EntityState.Modified;

                await applicationContext.SaveChangesAsync();
            }

            return photo;
        }
    }
}