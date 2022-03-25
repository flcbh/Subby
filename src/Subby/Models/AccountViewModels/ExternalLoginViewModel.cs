using System.ComponentModel.DataAnnotations;

namespace Subby.Web.New.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}