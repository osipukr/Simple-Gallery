using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Simply_Gallery.ViewModels
{
    public class AlbumViewModel
    {
        [Required(ErrorMessage = "Поле название альбома обязательно для ввода")]
        [Display(Name = "Название альбома")]
        [StringLength(20, MinimumLength = 4)]
        public string Name { get; set; }
    }

    public class PhotoViewModel
    {
        [Required(ErrorMessage = "Выберите фотографию")]
        [Display(Name = "Фотография")]
        public HttpPostedFileBase Image { get; set; }

        [ScaffoldColumn(false)]
        public int CurrentAlbumId { get; set; }
    }
}