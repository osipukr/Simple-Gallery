using System.Collections.Generic;
using System.Threading.Tasks;
using Simply_Gallery.Models.Gallery;
using Simply_Gallery.Repositories;

namespace Simply_Gallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _pictureRepository;

        public PhotoService(IPhotoRepository pictureRepository)
        {
            _pictureRepository = pictureRepository;
        }

        public async Task<IEnumerable<Photo>> GetPhotosAsync(int albumId)
        {
            return await _pictureRepository.GetPhotosAsync(albumId);
        }

        public async Task<Photo> GetPhotoAsync(int photoId)
        {
            return await _pictureRepository.GetPhotoAsync(photoId);
        }

        public async Task<Photo> AddPhotoAsync(Photo photo)
        {
            return await _pictureRepository.AddPhotoAsync(photo);
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            await _pictureRepository.DeletePhotoAsync(photoId);
        }

        public async Task<Photo> UpdatePhotoAsync(Photo photo)
        {
            return await _pictureRepository.UpdatePhotoAsync(photo);
        }
    }
}