using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Entities;
using Subby.Blazor.Maui.Models;

namespace Subby.Blazor.Maui.Controllers
{
    [Authorize]
    public class PoliciesController : Controller
    {
        private readonly IRepository _repository;

        public PoliciesController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("/policies/terms-of-service")]
        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet("[action]")]
        public IActionResult About()
        {
            return View();
        }


        [HttpGet("[action]")]
        public IActionResult Premium()
        {
            return View();
        }

        [HttpGet("[action]")]
        public IActionResult Benefits()
        {
            var user = _repository.Linq<User>().Include(x => x.Subscriptions).FirstOrDefault(x => x.Email == User.Identity.Name);
            var subscriptionExpiryDate =
                user.Subscriptions.OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
            var activeSubscription = subscriptionExpiryDate != null && subscriptionExpiryDate.ExpiryDate >= DateTime.Now;

            var benefits = _repository.Linq<Benefit>().Where(x => x.Active).ToList();

            var model = new SubscriptionViewModel
            {
                IsActive = activeSubscription,
                ExpiryDate = subscriptionExpiryDate?.ExpiryDate,
                Benefits = benefits
            };

            return View(model);
        }

        [HttpGet("/benefits/{slug}")]
        public IActionResult Sponsors(string slug)
        {
            var benefits = _repository.Linq<Benefit>()
                .Include(x => x.Sponsors).ThenInclude(x => x.Sponsor)
                .FirstOrDefault(x => x.Slug == slug);
            return View(benefits);
        }
    }
}