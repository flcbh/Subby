using Subby.Data;
using System.ComponentModel.DataAnnotations;

namespace SubbyNetwork.Models
{
    public class LoginViewModel
    {
        SubbynetworkContext db;
        public LoginViewModel()
        {
            db = new SubbynetworkContext();

        }


        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool LoginFailureHidden { get; set; } = true;

        public bool ValidateLogin(out string jwtToken)
        {
            try
            {
                var user = db.User.Where(u => u.UserName == Username).FirstOrDefault();

                if (user != null) //Username.Equals("flcbh@hotmail.com") && Password.Equals("Dayvid19"))
                {
                    if (user.PasswordHash == Password)
                    {
                        jwtToken = "ACC123456" + DateTime.Now.ToString();
                        return true;
                    }
                }

                //Not valid
                jwtToken = null;
                LoginFailureHidden = false;
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
