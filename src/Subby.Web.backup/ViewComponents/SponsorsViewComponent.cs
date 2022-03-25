using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.Extensions;
using Subby.Utilities.Interfaces;

namespace Subby.Web.ViewComponents
{
    public class SponsorsViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public SponsorsViewComponent(IRepository repository)
        {
            _repository = repository;
        }
        
        public Task<IViewComponentResult> InvokeAsync()
        {
            var sponsors = _repository.Linq<Sponsor>().Where(x => x.IsActive).ToList();
            return Task.FromResult((IViewComponentResult)View("Default", sponsors));
        }
    }
}