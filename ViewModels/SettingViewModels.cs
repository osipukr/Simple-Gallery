using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Simply_Gallery.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [StringLength(50, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпрадают")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeNameViewModel
    {
        [Display(Name = "Текущее имя пользователя")]
        public string CurrentUserName { get; set; }

        [Required(ErrorMessage = "Введите новое имя пользователя")]
        [Display(Name = "Новое имя пользователя")]
        [StringLength(50, MinimumLength = 4)]
        public string NewUserName { get; set; }
    }

    public class ChangeEmailViewModel
    {
        [Display(Name = "Текущая почта")]
        public string CurrentEmail { get; set; }

        [Required(ErrorMessage = "Поле новая почта обязательно для ввода")]
        [Display(Name = "Новая почта")]
        [EmailAddress]
        public string NewEmail { get; set; }
    }

    public class ChangeAvatarViewModel
    {
        [Required(ErrorMessage = "Выберите фотографию")]
        [Display(Name = "Фотография пользователя")]
        public HttpPostedFileBase Image { get; set; }
    }
}