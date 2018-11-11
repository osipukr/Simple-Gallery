using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Simply_Gallery.ViewModels
{
    public class PhotoViewModel
    {
        [Required(ErrorMessage = "Выберите фотографию")]
        [Display(Name = "Фотография")]
        public HttpPostedFileBase Image { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int CurrentAlbumId { get; set; }
    }
}