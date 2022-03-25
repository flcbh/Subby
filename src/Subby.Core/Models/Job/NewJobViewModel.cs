using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Subby.Core.Models.Job
{
    public class NewJobViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Location { get; set; }
        [DisplayName("Completion Date")]
        public string DeadLine { get; set; }

        public string Latitude { get; set; }
        
        public string Longitude { get; set; }
        [DisplayName("Website")]
        public string ExternalLink { get; set; }
        
        [Required]
        public string Postcode { get; set; }
        
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFeatured { get; set; } = true;

        public string Budget { get; set; }
        [DisplayName("Contract Type")]
        public string ContractType { get; set; }
        [DisplayName("Trade")]
        public int TradeId { get; set; }
        public bool IsFilled { get; set; }
        public string Media { get; set; }
        
        public IFormFile File { get; set; }
    }
}