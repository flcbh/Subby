using FluentValidation;
using Subby.Core.Models.Job;
using Subby.Web.ApiModels;

namespace Subby.Web.Validations
{
    public class NewJobValidator : AbstractValidator<NewJobViewModel>
    {
        public NewJobValidator()
        {
            RuleFor(vm => vm.Title).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.DeadLine).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.TradeId).NotEmpty().WithMessage("This field is required");
            RuleFor(vm => vm.Description).NotEmpty().WithMessage("This field is required");
        }
    }
}