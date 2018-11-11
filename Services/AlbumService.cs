using System.Collections.Generic;
using System.Threading.Tasks;
using Simply_Gallery.Models.Gallery;
using Simply_Gallery.Repositories;

namespace Simply_Gallery.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;

        public AlbumService(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<IEnumerable<Album>> GetAlbumsAsync(string userId)
        {
            return await _albumRepository.GetAlbumsAsync(userId);
        }

        public async Task<Album> GetAlbumAsync(int albumId)
        {
            return await _albumRepository.GetAlbumAsync(albumId);
        }

        public async Task<Album> GetAlbumAsync(int albumId, string userId)
        {
            var album = await _albumRepository.GetAlbumAsync(albumId);

            if(album != null)
            {
                return album.UserId == userId ? album : null;
            }

            return null;
        }

        public async Task<Album> GetAlbumAsync(string albumName)
        {
            return await _albumRepository.GetAlbumAsync(albumName);
        }

        public async Task<Album> GetAlbumAsync(string albumName, string userId)
        {
            var album = await _albumRepository.GetAlbumAsync(albumName);

            if(album != null)
            {
                return album.UserId == userId ? album : null;
            }

            return null;
        }

        public async Task<Album> AddAlbumAsync(Album album)
        {
            return await _albumRepository.AddAlbumAsync(album);
        }

        public async Task DeleteAlbumAsync(int albumId)
        {
            await _albumRepository.DeleteAlbumAsync(albumId);
        }

        public async Task<Album> UpdateAlbumAsync(Album album)
        {
            return await _albumRepository.UpdateAlbumAsync(album);
        }
    }
}