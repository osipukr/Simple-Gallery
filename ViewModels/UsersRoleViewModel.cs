using Simply_Gallery.Models;
using System.Collections.Generic;

namespace Simply_Gallery.ViewModels
{
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