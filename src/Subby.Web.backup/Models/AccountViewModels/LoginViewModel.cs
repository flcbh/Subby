using System.ComponentModel.DataAnnotations;

namespace Subby.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")] public bool RememberMe { get; set; } = true;
        public string PushToken { get; set; }
    }
}