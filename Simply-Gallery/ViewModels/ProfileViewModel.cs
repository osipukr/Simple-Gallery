using System.Collections.Generic;

namespace Simply_Gallery.ViewModels
{
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