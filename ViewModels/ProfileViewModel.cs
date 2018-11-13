using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simply_Gallery.ViewModels
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public ICollection<string> Roles { get; set; }
        public byte[] UserImage { get; set; }

    }
}