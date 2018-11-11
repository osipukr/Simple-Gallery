using System.Web.Mvc;

namespace Simply_Gallery.ViewModels
{
    public class PhotoViewModel
    {
        public string MimeType { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CurrentAlbumId { get; set; }
    }
}