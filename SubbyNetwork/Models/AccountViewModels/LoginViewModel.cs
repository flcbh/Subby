﻿using Microsoft.AspNetCore.Identity;
using Subby.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace SubbyNetwork.Models.AccountViewModels
{
    public class LoginViewModel
    {
        SubbynetworkContext db;
        //private readonly SignInManager<User> _signInManager;

        public LoginViewModel()
        {
            db = new SubbynetworkContext();
        }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool LoginFailureHidden { get; set; } = true;
        public bool RememberMe { get; private set; } = true;

        public bool ValidateLoginAsync(out string jwtToken)
        {
            try
            {
                var user = db.User.Where(u => u.UserName == Email).FirstOrDefault();

                //var result =  _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: false);

                if (user != null)
                {
                    //if (result.Result.Succeeded)
                    //{
                        jwtToken = "ACC123456" + DateTime.Now.ToString();
                        return true;
                    //}
                }

                //Not valid
                jwtToken = null;
                LoginFailureHidden = false;
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
