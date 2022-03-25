using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Subby.Core.Extensions;
using Subby.Utilities.Interfaces;

namespace Subby.Web.New.ViewComponents
{
    public class AnnualPriceViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public AnnualPriceViewComponent(IRepository repository)
        {
            _repository = repository;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("ANNUAL_PRICE", 49m);
            return Task.FromResult((IViewComponentResult)View("Default", premiumPrice));
        }
    }
}