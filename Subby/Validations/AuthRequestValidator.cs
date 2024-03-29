using FluentValidation;
using Subby.ApiModels;

namespace Subby.Validations
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