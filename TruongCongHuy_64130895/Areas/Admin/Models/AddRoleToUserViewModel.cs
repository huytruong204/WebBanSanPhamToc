namespace TruongCongHuy_64130895.Areas.Admin.Models
{
    public class AddRoleToUserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public string SelectedRole { get; set; }
    }
}
