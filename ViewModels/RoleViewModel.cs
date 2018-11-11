using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Поле название обязательно для заполнения")]
        [Display(Name = "Название")]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [StringLength(30)]
        public string Description { get; set; }
    }
}