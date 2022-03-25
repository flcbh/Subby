using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;

namespace Subby.ViewComponents
{
    public class MemberBenefitViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public MemberBenefitViewComponent(IRepository repository)
        {
            _repository = repository;
        }

        public System.Security.Principal.IPrincipal GetUser()
        {
            return User;
        }

        public Task<IViewComponentResult> InvokeAsync(System.Security.Principal.IPrincipal user)
        {
            var haveActiveSubscription = false;
            if (user.Identity.IsAuthenticated)
            {
                var user1 = _repository.Linq<User>().Include(x => x.Subscriptions).FirstOrDefault(x => x.Email == User.Identity.Name);
                var subscriptionExpiryDate =
                    user1.Subscriptions.OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
                haveActiveSubscription = subscriptionExpiryDate != null && subscriptionExpiryDate.ExpiryDate >= DateTime.Now;
            }

            return Task.FromResult((IViewComponentResult)View("Default", haveActiveSubscription));
        }
    }
}