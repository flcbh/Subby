using LastContent.Utilities.Caching;
using LastContent.Utilities.GeoCoordinate;
using Subby.Data;
using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        private SubbynetworkContext db;
        private readonly IGeocoder geocoder;

        public RegisterViewModel()
        {
            geocoder = new GoogleGeocoder(null, null, "AIzaSyA2LbhHIYOz1EWe_ld3uBLhFMib6KkUtpk");
            this.db = new SubbynetworkContext();
        }

        [Required]
        [Display(Name = "Forename")]
        public string Forename { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public bool IsTrader { get; set; }

        [Required]
        public string Postcode { get; set; }
        public string PushToken { get; set; }

        public bool Save(RegisterViewModel model)
        {
            if (model == null) return false;

            var verify = db.User.Where(u => u.Email == model.Email).Any(); 
            
            if (verify) throw new InvalidOperationException("User already registered.");

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.Forename,
                LastName = model.Surname,
                IsTrader = model.IsTrader,
                TradePostcode = model.Postcode,
                Role = "User",
                PushToken = model.PushToken
            };

            if (!string.IsNullOrEmpty(model.Postcode))
            {
                try
                {
                    var geoLocation = geocoder.FindCoordinates(model.Postcode, "GB");
                    user.Latitude = geoLocation.Latitude.ToString();
                    user.Longitude = geoLocation.Longitude.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }

            //send confirmation email 

            db.User.Add(user);

            return true;
        }
    }
}