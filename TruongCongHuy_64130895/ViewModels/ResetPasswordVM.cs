using System.ComponentModel.DataAnnotations;

namespace TruongCongHuy_64130895.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? Token { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp")]
        [DataType(DataType.Password)]
        public string? ConfirmdPassword { get; set; }

    }
}
