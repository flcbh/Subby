using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}