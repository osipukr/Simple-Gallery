using Simply_Gallery.Models.Gallery;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Simply_Gallery.Interfaces
{
    public interface IPhotoService
    {
        Task<IEnumerable<Photo>> GetPhotosAsync(Expression<Func<Photo, bool>> predicate);
        Task<Photo> GetPhotoAsync(Expression<Func<Photo, bool>> predicate);
        Task<Photo> AddPhotoAsync(Photo photo);
        Task<Photo> UpdatePhotoAsync(Photo photo);
        Task DeletePhotoAsync(Expression<Func<Photo, bool>> predicate);
        Task<bool> IsPhotoFindAsync(Expression<Func<Photo, bool>> predicate);
    }
}