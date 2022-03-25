using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;

namespace Subby.Web.ViewComponents
{
    public class MemberBenefitViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public MemberBenefitViewComponent(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<IViewComponentResult> InvokeAsync()
        {
            var haveActiveSubscription = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = _repository.Linq<User>().Include(x => x.Subscriptions).FirstOrDefault(x => x.Email == User.Identity.Name);
                var subscriptionExpiryDate =
                    user.Subscriptions.OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
                haveActiveSubscription = subscriptionExpiryDate != null && subscriptionExpiryDate.ExpiryDate >= DateTime.Now;
            }

            return Task.FromResult((IViewComponentResult)View("Default", haveActiveSubscription));
        }
    }
}