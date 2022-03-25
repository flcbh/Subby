using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Subby.Web.Models.AdminViewModels
{
    public class SponsorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public bool IsActive { get; set; } = true; 
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public IFormFile File { get; set; }
        [Required]
        [Display(Name = "Benefits")]
        public List<int> Benefits { get; set; }
    }
}