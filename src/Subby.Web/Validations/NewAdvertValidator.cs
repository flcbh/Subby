using FluentValidation;
using Subby.Web.Models.AdvertViewModels;

namespace Subby.Web.Validations
{
    public class NewAdvertValidator : AbstractValidator<NewAdvertViewModel>
    {
        public NewAdvertValidator()
        {
            RuleFor(vm => vm.Title).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.Location).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.CategoryId).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.Description).NotEmpty().WithMessage("This field is required");
        }
    }
}