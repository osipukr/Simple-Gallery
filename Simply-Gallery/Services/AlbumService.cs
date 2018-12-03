using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IRepository<Album> _albumRepository;

        public AlbumService(IRepository<Album> albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<IEnumerable<Album>> GetAlbumsAsync(Expression<Func<Album, bool>> predicate)
        {
            return await _albumRepository.GetAllAsync(predicate);
        }

        public async Task<Album> GetAlbumAsync(Expression<Func<Album, bool>> predicate)
        {
            return await _albumRepository.GetAsync(predicate);
        }

        public async Task<Album> AddAlbumAsync(Album album)
        {
            return await _albumRepository.AddAsync(album);
        }

        public async Task DeleteAlbumAsync(Expression<Func<Album, bool>> predicate)
        {
            await _albumRepository.DeleteAsync(predicate);
        }

        public async Task<Album> UpdateAlbumAsync(Album album)
        {
            return await _albumRepository.UpdateAsync(album);
        }

        public async Task<bool> IsAlbumFindAsync(Expression<Func<Album, bool>> predicate)
        {
            return await _albumRepository.IsFindAsync(predicate);
        }
    }
}