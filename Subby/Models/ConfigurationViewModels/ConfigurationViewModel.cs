using System.ComponentModel.DataAnnotations;

namespace Subby.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        [Required]
        public string Value { get; set; }
        public int Id { get; set; }
    }
}