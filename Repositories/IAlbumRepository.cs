using Simply_Gallery.Models.Gallery;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simply_Gallery.Repositories
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<Album>> GetAlbumsAsync(string userId);
        Task<Album> GetAlbumAsync(int albumId);
        Task<Album> GetAlbumAsync(string albumName);
        Task<Album> AddAlbumAsync(Album album);
        Task DeleteAlbumAsync(int albumId);
        Task<Album> UpdateAlbumAsync(Album album);
    }
}