using Simply_Gallery.Models.Gallery;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simply_Gallery.Interfaces
{
    public interface IAlbumService
    {
        Task<IEnumerable<Album>> GetAlbumsAsync(string userId);
        Task<Album> GetAlbumAsync(int albumId);
        Task<Album> GetAlbumAsync(int albumId, string userId);
        Task<Album> GetAlbumAsync(string albumName);
        Task<Album> GetAlbumAsync(string albumName, string userId);
        Task<Album> AddAlbumAsync(Album album);
        Task DeleteAlbumAsync(int albumId);
        Task<Album> UpdateAlbumAsync(Album album);
    }
}