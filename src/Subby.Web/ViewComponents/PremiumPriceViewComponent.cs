using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Subby.Core.Extensions;
using Subby.Utilities.Interfaces;

namespace Subby.Web.ViewComponents
{
    public class PremiumPriceViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public PremiumPriceViewComponent(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<IViewComponentResult> InvokeAsync()
        {
            var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("PREMIUM_PRICE", 4.99m);
            return Task.FromResult((IViewComponentResult)View("Default", premiumPrice));
        }
    }
}