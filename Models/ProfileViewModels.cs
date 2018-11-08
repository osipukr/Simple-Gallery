using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Simply_Gallery.Models
{
    public class AlbumModel
    {
        [Required(ErrorMessage = "Поле название альбома обязательно для ввода")]
        [Display(Name = "Название альбома")]
        [StringLength(20, MinimumLength = 4)]
        public string Name { get; set; }
    }

    public class PhotoModel
    {
        public string MimeType { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CurrentAlbumId { get; set; }
    }
}