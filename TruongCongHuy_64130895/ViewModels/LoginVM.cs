using System.ComponentModel.DataAnnotations;

namespace TruongCongHuy_64130895.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email hoặc tên người dùng là bắt buộc.")]
        public string? EmailOrUsername { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
