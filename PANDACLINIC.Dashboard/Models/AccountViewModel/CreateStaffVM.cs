using System.ComponentModel.DataAnnotations;

namespace PANDACLINIC.Dashboard.Models.AccountViewModel
{
    public class CreateStaffVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
