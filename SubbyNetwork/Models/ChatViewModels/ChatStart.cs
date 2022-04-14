using Subby.Data;
using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models.ChatViewModels
{
    public class ChatStart
    {
        public int AdvertId { get; set; }
        [Required]
        public string Message { get; set; }
        public Advert Advert { get; set; }
    }
}