using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Subby.Web.New.Models.ReviewViewModels
{
    public class WriteAReviewViewModel
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        [Required]
        [Display(Name = "Write a review")]
        public string Review { get; set; }
        [Required]
        [Display(Name = "Date work completed")]
        public DateTime DateCompleted { get; set; }
        [Required]
        public int Tidiness { get; set; }
        [Required]
        public int Reliability { get; set; }
        [Required]
        public int Courtesy { get; set; }
    }
}