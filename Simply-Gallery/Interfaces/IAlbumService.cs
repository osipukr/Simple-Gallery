using Simply_Gallery.Models.Gallery;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Simply_Gallery.Interfaces
{
    public interface IAlbumService
    {
        Task<IEnumerable<Album>> GetAlbumsAsync(Expression<Func<Album, bool>> predicate);
        Task<Album> GetAlbumAsync(Expression<Func<Album, bool>> predicate);
        Task<Album> AddAlbumAsync(Album album);
        Task<Album> UpdateAlbumAsync(Album album);
        Task DeleteAlbumAsync(Expression<Func<Album, bool>> predicate);
        Task<bool> IsAlbumFindAsync(Expression<Func<Album, bool>> predicate);
    }
}