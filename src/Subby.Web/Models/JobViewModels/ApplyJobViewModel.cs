using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Subby.Core.Entities;

namespace Subby.Web.Models.JobViewModels
{
    public class ApplyJobViewModel
    {
        public Job Job { get; set; }
        public int JobId { get; set; }
        [Required]
        [Display(Name = "Forename")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Display(Name = "Eligible to work in the UK")]
        public bool EligibleToWorkInUk { get; set; }
        [Display(Name = "Eligible to work in the EU")]
        public bool EligibleToWorkInEu { get; set; }
        [Required]
        public string Estimation { get; set; }
        [Required]
        public string Quote { get; set; }
        public IFormFile File { get; set; }
    }
}