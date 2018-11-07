using System.Collections.Generic;

namespace Simply_Gallery.Models
{
    public class Picture
    {
        public int PictureId { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }
    }

    public class Album
    {
        public int AlbumId { get; set; }

        public string Name { get; set; }

        //1й вариант
        public virtual ICollection<Picture> Pictures { get; set; }

        //2й вариант
        //public int PictureId { get; set; }
    }

    public class ProfileModel
    {
        public string UserId { get; set; }

        // 1й вариант
        public virtual ICollection<Album> Albums { get; set; }

        // 2й вариант
        //public int AlbumId { get; set; }
    }
}