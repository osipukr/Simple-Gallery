using Simply_Gallery.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Поле название обязательно для заполнения")]
        [Display(Name = "Название")]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [StringLength(30)]
        public string Description { get; set; }
    }

    public class EditRoleViewModel
    {
        [Display(Name = "Название")]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [StringLength(30)]
        public string Description { get; set; }
    }

    public class UsersRoleViewModel
    {
        public string Name { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public UsersRoleViewModel()
        {
            Users = new List<ApplicationUser>();
        }
    }
}