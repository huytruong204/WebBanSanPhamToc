using System.ComponentModel.DataAnnotations;

namespace TruongCongHuy_64130895.ViewModels
{
	public class ForgotPasswordVM
	{
		[Required]
		[DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? Email { get; set; }
	}
}
