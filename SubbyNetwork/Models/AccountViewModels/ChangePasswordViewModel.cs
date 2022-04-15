using Subby.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubbyNetwork.Models.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        private SubbynetworkContext db;

        public ChangePasswordViewModel()
        {
            db = new SubbynetworkContext();
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool Save(ChangePasswordViewModel model)
        {
            if (model == null) return false;

            User user = db.User.Where(u => u.Email == model.Email).FirstOrDefault();

            if (user == null) throw new InvalidOperationException("User not exist.");

            if(model.CurrentPassword != user.PasswordHash) throw new InvalidOperationException("Current password is not correct.");

            user.PasswordHash = model.Password;

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User> entityEntry = db.User.Update(user);

            return true;
        }
    }
}
