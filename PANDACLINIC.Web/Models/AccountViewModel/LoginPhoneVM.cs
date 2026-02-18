using System.ComponentModel.DataAnnotations;

namespace PANDACLINIC.Web.Models.AccountViewModel
{
    public class LoginPhoneVM
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
