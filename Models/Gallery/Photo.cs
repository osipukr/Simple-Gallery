namespace Simply_Gallery.Models.Gallery
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public byte[] Image { get; set; }
        public string ImageMimeType { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }
    }
}