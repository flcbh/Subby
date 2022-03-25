using System.ComponentModel.DataAnnotations;

namespace Subby.Blazor.Maui.Models.AdminViewModels
{
    public class BenefitViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public bool Active { get; set; } = true;
    }
}