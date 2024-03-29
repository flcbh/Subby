using System.ComponentModel.DataAnnotations;

namespace Subby.Web.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}