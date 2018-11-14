using System.Collections.Generic;

namespace Simply_Gallery.Models.Gallery
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        public IEnumerable<Photo> Photos { get; set; }
    }
}