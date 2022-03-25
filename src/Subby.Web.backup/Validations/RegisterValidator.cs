using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Subby.Core.Entities;
using Subby.Web.Models.AccountViewModels;

namespace Subby.Web.Validations
{
    public class RegisterValidator  : AbstractValidator<RegisterViewModel>
    {
        private readonly UserManager<User> _userManager;
        
        public RegisterValidator(UserManager<User> userManager)
        {
            _userManager = userManager;
            RuleFor(vm => vm.Email).Must(x => !IsDuplicate(x)).WithMessage("Your email address is already registered with us");
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email address is required");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("Please enter a valid email address");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(vm => vm.ConfirmPassword).NotEmpty().WithMessage("Please confirm your password");
            RuleFor(vm => vm.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");
            RuleFor(vm => vm.Forename).NotEmpty().WithMessage("Please enter your forename");
            RuleFor(vm => vm.Surname).NotEmpty().WithMessage("Please enter your surname");
            RuleFor(vm => vm.Postcode).NotEmpty().WithMessage("This field is required");
        }

        private bool IsDuplicate(string email)
        {
            var result = _userManager.FindByEmailAsync(email).Result;
            return result != null;
        }
    }
}