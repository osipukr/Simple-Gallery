using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле имя пользователя обязательно для ввода")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле пароль обязательно для ввода")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}