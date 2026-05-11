using System.ComponentModel.DataAnnotations;

namespace TruongCongHuy_64130895.ViewModels
{
    public class EditCustomerViewModel
    {
        public string Id { get; set; } // ID của người dùng

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên không vượt quá 100 ký tự")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "Địa chỉ không vượt quá 100 ký tự")]
		[DataType(DataType.Password)]
		public string? OldPassword { get; set; }

		[DataType(DataType.Password)]
		public string? NewPassword { get; set; }
		public string? Address { get; set; }

        public string? Avatar { get; set; } 
    }
}
