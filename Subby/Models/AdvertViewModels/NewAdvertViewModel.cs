using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Subby.Core.Entities;

namespace Subby.Models.AdvertViewModels
{
    public class NewAdvertViewModel
    {
        public int Id { get; set; }
        public string ExternalLink { get; set; }
        public string Postcode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Condition { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Files { get; set; }
        public string Media { get; set; }
        public bool IsSold { get; set; }
        public bool IsFree { get; set; }
        public List<Media> MediaCollection { get; set; }
    }
}