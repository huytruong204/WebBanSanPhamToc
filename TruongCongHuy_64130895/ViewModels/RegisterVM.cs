using System.ComponentModel.DataAnnotations;

namespace TruongCongHuy_64130895.ViewModels
{
    public class RegisterVM
    {
        [StringLength(100)]
        [MaxLength(250)]
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng.")]
        public string? UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [Display(Name = "Confirmd Password")]
		[DataType(DataType.Password)]
		public string? ConfirmdPassword { get; set;}
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

    }
}
