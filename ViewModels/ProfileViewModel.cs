using System.Collections.Generic;

namespace Simply_Gallery.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public ICollection<string> UserRoles { get; set; }
        public byte[] UserImage { get; set; }
    }
}