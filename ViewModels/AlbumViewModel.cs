using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.ViewModels
{
    public class AlbumViewModel
    {
        [Required(ErrorMessage = "Поле название альбома обязательно для ввода")]
        [Display(Name = "Название альбома")]
        [StringLength(20, MinimumLength = 4)]
        public string Name { get; set; }
    }
}