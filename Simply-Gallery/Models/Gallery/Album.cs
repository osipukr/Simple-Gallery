using System;
using System.Linq;

namespace Simply_Gallery.Models.Gallery
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }

        public IQueryable<Photo> Photos { get; set; }
    }
}