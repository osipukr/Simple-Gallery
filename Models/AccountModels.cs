using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Поле имя пользователя обязательно для ввода")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле пароль обязательно для ввода")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Поле почта обязательно для ввода")]
        [Display(Name = "Почта")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле имя пользователя обязательно для ввода")]
        [Display(Name = "Имя пользователя")]
        [StringLength(50, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле пароль обязательно для ввода")]
        [Display(Name = "Пароль")]
        [StringLength(50, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Повтор пароля")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпрадают")]
        public string ConfirmPassword { get; set; }
    }
}