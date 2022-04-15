using Subby.Data;
using SubbyNetwork.Extensions;
using SubbyNetwork.Interfaces;
using SubbyNetwork.Services;
using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        SubbynetworkContext _db;
        private readonly SendEmail _sendEmail;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ForgotPasswordViewModel()
        {
            _db = new SubbynetworkContext();
            _sendEmail = new SendEmail();
        }
        public bool SendPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = _db.User.Where(u => u.UserName == model.Email).FirstOrDefault();

                if (user != null) 
                {
                    var body = string.Format("Your password: {0}", user.PasswordHash);

                    _sendEmail.Send(user.Email, string.Format("{0} {1}", user.FirstName, user.LastName), "Important", body);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}