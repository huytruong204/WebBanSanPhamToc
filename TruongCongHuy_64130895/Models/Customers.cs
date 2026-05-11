using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TruongCongHuy_64130895.Models
{
    public class Customers : IdentityUser
    {
        [StringLength(100)]
        [MaxLength(250)]
        [Required]
        public string? FullName { get; set; }
        [StringLength(100)]
        [MaxLength(250)]
        public string? Address { get; set; }
        public string? avatar { get; set; }
        
    }
}
