using System.ComponentModel.DataAnnotations;

namespace Simply_Gallery.Models
{
    public class RoleModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Name role")]
        public string Name { get; set; }

        [Display(Name = "Description role")]
        public string Description { get; set; }
    }

    //public class CreateRoleModel
    //{
    //    [Display(Name = "Name role")]
    //    public string Name { get; set; }

    //    [Display(Name = "Description role")]
    //    public string Description { get; set; }
    //}
}