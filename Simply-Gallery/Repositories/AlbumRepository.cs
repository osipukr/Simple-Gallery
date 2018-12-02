using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly IPhotoRepository _photoRepository;

        public AlbumRepository()
        {
        }

        public AlbumRepository(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<IQueryable<Album>> GetAlbumsAsync(string userId)
        {
            var result = new List<Album>();

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Albums.Where(x => x.UserId == userId).ToListAsync();

                foreach (var album in result)
                {
                    album.Photos = await _photoRepository.GetPhotosAsync(album.Id);
                }
            }

            return result.AsQueryable();
        }

        public async Task<Album> GetAlbumAsync(int albumId)
        {
            Album result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Albums.FirstOrDefaultAsync(x => x.Id == albumId);

                if (result != null)
                {
                    result.Photos = await _photoRepository.GetPhotosAsync(result.Id);
                }
            }

            return result;
        }

        public async Task<Album> GetAlbumAsync(string albumName)
        {
            Album result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Albums.FirstOrDefaultAsync(x => x.Name == albumName);

                if (result != null)
                {
                    result.Photos = await _photoRepository.GetPhotosAsync(result.Id);
                }
            }

            return result;
        }

        public async Task<Album> AddAlbumAsync(Album album)
        {
            Album result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = applicationContext.Albums.Add(album);
                await applicationContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task DeleteAlbumAsync(int albumId)
        {
            using (var applicationContext = new ApplicationContext())
            {
                var album = await applicationContext.Albums.FirstOrDefaultAsync(x => x.Id == albumId);

                applicationContext.Entry(album).State = EntityState.Deleted;

                await applicationContext.SaveChangesAsync();
            }
        }

        public async Task<Album> UpdateAlbumAsync(Album album)
        {
            using (var applicationContext = new ApplicationContext())
            {
                applicationContext.Entry(album).State = EntityState.Modified;

                await applicationContext.SaveChangesAsync();
            }

            return album;
        }
    }
}