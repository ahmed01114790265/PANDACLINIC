using System.ComponentModel.DataAnnotations;

namespace PANDACLINIC.Web.Models.AccountViewModel
{
    public class LoginPhoneVM : IValidatableObject
    {
        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        [Required]
        public string AccountType { get; set; } = "Customer";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AccountType == "Customer" && string.IsNullOrWhiteSpace(PhoneNumber) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Phone Number is required", new[] { nameof(PhoneNumber) });
            }

            if ((AccountType == "Admin" || AccountType == "Staff") && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required", new[] { nameof(Email) });
            }
        }
    }
}
