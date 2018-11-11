using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Simply_Gallery.Common;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        public AlbumRepository()
        {
        }

        public async Task<IEnumerable<Album>> GetAlbumsAsync(string userId)
        {
            var result = new List<Album>();

            using (var galleryContext = new GalleryContext())
            {
                result = await galleryContext.Albums.Where(x => x.UserId == userId).ToListAsync();
            }

            return result;
        }

        public async Task<Album> GetAlbumAsync(int albumId)
        {
            Album result = null;

            using (var galleryContext = new GalleryContext())
            {
                result = await galleryContext.Albums.FirstOrDefaultAsync(x => x.AlbumId == albumId);
            }

            return result;
        }

        public async Task<Album> GetAlbumAsync(string albumName)
        {
            Album result = null;

            using (var galleryContext = new GalleryContext())
            {
                result = await galleryContext.Albums.FirstOrDefaultAsync(x => x.Name == albumName);
            }

            return result;
        }

        public async Task<Album> AddAlbumAsync(Album album)
        {
            Album result = null;

            using (var galleryContext = new GalleryContext())
            {
                result = galleryContext.Albums.Add(album);
                await galleryContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task DeleteAlbumAsync(int albumId)
        {
            using (var galleryContext = new GalleryContext())
            {
                var album = await galleryContext.Albums.FirstOrDefaultAsync(x => x.AlbumId == albumId);

                galleryContext.Entry(album).State = EntityState.Deleted;

                await galleryContext.SaveChangesAsync();
            }
        }

        public async Task<Album> UpdateAlbumAsync(Album album)
        {
            using (var galleryContext = new GalleryContext())
            {
                galleryContext.Entry(album).State = EntityState.Modified;

                await galleryContext.SaveChangesAsync();
            }

            return album;
        }
    }
}