using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models.ConfigurationViewModels
{
    public class ConfigurationViewModel
    {
        [Required]
        public string Value { get; set; }
        public int Id { get; set; }
    }
}