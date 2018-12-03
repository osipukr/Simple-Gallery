using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IRepository<Photo> _photoRepository;

        public PhotoService(IRepository<Photo> photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<IEnumerable<Photo>> GetPhotosAsync(Expression<Func<Photo, bool>> predicate)
        {
            return await _photoRepository.GetAllAsync(predicate);
        }

        public async Task<Photo> GetPhotoAsync(Expression<Func<Photo, bool>> predicate)
        {
            return await _photoRepository.GetAsync(predicate);
        }

        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            return await _photoRepository.AddAsync(photo);
        }

        public async Task DeletePhotoAsync(Expression<Func<Photo, bool>> predicate)
        {
            await _photoRepository.DeleteAsync(predicate);
        }

        public async Task<Photo> UpdatePhotoAsync(Photo photo)
        {
            return await _photoRepository.UpdateAsync(photo);
        }

        public async Task<bool> IsPhotoFindAsync(Expression<Func<Photo, bool>> predicate)
        {
            return await _photoRepository.IsFindAsync(predicate);
        }
    }
}