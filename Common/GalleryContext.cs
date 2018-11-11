using Simply_Gallery.Models.Gallery;
using System.Data.Entity;

namespace Simply_Gallery.Common
{
    public class GalleryContext : DbContext
    {
        public GalleryContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
    }
}