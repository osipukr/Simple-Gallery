using System.Collections.Generic;
using System.Threading.Tasks;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<IEnumerable<Photo>> GetPhotosAsync(int albumId)
        {
            return await _photoRepository.GetPhotosAsync(albumId);
        }

        public async Task<Photo> GetPhotoAsync(int photoId)
        {
            return await _photoRepository.GetPhotoAsync(photoId);
        }

        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            return await _photoRepository.AddPhotoAsync(photo);
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            await _photoRepository.DeletePhotoAsync(photoId);
        }

        public async Task<Photo> UpdatePhotoAsync(Photo photo)
        {
            return await _photoRepository.UpdatePhotoAsync(photo);
        }
    }
}