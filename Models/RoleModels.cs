using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.Models
{
    public class RoleModel
    {
        public string Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    public class RoleViewModel
    {
        public string Name { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public RoleViewModel()
        {
            Users = new List<ApplicationUser>();
        }
    }
}