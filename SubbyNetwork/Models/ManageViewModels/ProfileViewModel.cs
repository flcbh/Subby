using Microsoft.AspNetCore.Http;
using Subby.Data;
using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models.ManageViewModels
{
    public class ProfileViewModel
    {
        private SubbynetworkContext db;

        public ProfileViewModel()
        {
            db = new SubbynetworkContext();
        }


        [Required]
        [Display(Name = "First name")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string Lastname { get; set; }
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }

        public bool MarketingOptIn { get; set; }

        public string Bio { get; set; }

        public bool IsTrader { get; set; }

        [Display(Name = "Company name")]
        public string TradeName { get; set; }
        [Display(Name = "Address Line 1")]
        public string TradeAddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string TradeAddressLine2 { get; set; }
        [Display(Name = "Town/City")]
        public string TradeCity { get; set; }
        [Display(Name = "Postcode")]
        public string TradePostcode { get; set; }
        [Display(Name = "Distance willing to travel")]
        public decimal TradeDistance { get; set; }

        [Display(Name = "Trades")] public List<int> Trades { get; set; } = new List<int>();

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool ShowExternalJobs { get; set; }

        public IFormFile File { get; set; }
        public string Avatar { get; set; }


        public bool Save(ProfileViewModel model)
        {
            return true;
        }


    }
}