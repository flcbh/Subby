using System.ComponentModel.DataAnnotations;
using Subby.Core.Entities;

namespace Subby.Web.New.Models.ChatViewModels
{
    public class ChatStart
    {
        public int AdvertId { get; set; }
        [Required]
        public string Message { get; set; }
        public Advert Advert { get; set; }
    }
}