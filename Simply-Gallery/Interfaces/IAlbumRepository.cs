using Simply_Gallery.Models.Gallery;
using System.Linq;
using System.Threading.Tasks;

namespace Simply_Gallery.Interfaces
{
    public interface IAlbumRepository
    {
        Task<IQueryable<Album>> GetAlbumsAsync(string userId);
        Task<Album> GetAlbumAsync(int albumId);
        Task<Album> GetAlbumAsync(string albumName);
        Task<Album> AddAlbumAsync(Album album);
        Task DeleteAlbumAsync(int albumId);
        Task<Album> UpdateAlbumAsync(Album album);
    }
}