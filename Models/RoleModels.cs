using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.Models
{
    public class RoleViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
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