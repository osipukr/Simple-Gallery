using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Simply_Gallery.ViewModels
{
    public class CreateAlbumViewModel
    {
        [Required(ErrorMessage = "Поле название альбома обязательно для ввода")]
        [Display(Name = "Название альбома")]
        [StringLength(20, MinimumLength = 4)]
        public string Name { get; set; }

        [Display(Name = "Описание альбома")]
        [StringLength(40)]
        public string Description { get; set; }
    }

    public class EditAlbumViewModel
    {
        [ScaffoldColumn(false)]
        public string OldName { get; set; }

        [ScaffoldColumn(false)]
        public string OldDescription { get; set; }

        [Display(Name = "Название альбома")]
        [StringLength(20, MinimumLength = 4)]
        public string NewName { get; set; }

        [Display(Name = "Описание альбома")]
        [StringLength(40)]
        public string NewDescription { get; set; }
    }

    public class PhotoViewModel
    {
        [Required(ErrorMessage = "Выберите фотографию")]
        [Display(Name = "Фотография")]
        public HttpPostedFileBase Image { get; set; }

        [ScaffoldColumn(false)]
        public int CurrentAlbumId { get; set; }
    }

    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool UserIsImage { get; set; }
        public int CountAlbum { get; set; }
        public ICollection<string> UserRoles { get; set; }

        public ProfileViewModel()
        {
            UserRoles = new List<string>();
        }
    }
}