using System.ComponentModel.DataAnnotations;

namespace Subby.Models.FIrebaseViewModels
{
    public class PushViewModel
    {
        [Required]
        public string Message { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}