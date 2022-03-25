using FluentValidation;
using Subby.Web.New.ApiModels;

namespace Subby.Web.New.Validations
{
    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidator()
        {
            RuleFor(vm => vm.AccessKey).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.AccessSecret).NotEmpty().WithMessage("This field is required");
        }
    }
}