using System.Linq;

namespace Simply_Gallery.Models.Gallery
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public IQueryable<Photo> Photos { get; set; }
    }
}