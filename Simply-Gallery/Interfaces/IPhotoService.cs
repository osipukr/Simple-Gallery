using Simply_Gallery.Models.Gallery;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simply_Gallery.Interfaces
{
    public interface IPhotoService
    {
        Task<IEnumerable<Photo>> GetPhotosAsync(int albumId);
        Task<Photo> GetPhotoAsync(int photoId);
        Task<Photo> AddPhotoAsync(Photo photo);
        Task DeletePhotoAsync(int photoId);
        Task<Photo> UpdatePhotoAsync(Photo photo);
    }
}