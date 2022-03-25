using System.ComponentModel.DataAnnotations;

namespace Subby.Blazor.Maui.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}