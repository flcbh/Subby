using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Extensions;
using Subby.Utilities.Interfaces;

namespace Subby.Web.ViewComponents
{
    public class NewJobCountViewComponent : ViewComponent
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;

        public NewJobCountViewComponent(IRepository repository, IDomainEvents events)
        {
            _repository = repository;
            _events = events;
        }
        
        public Task<IViewComponentResult> InvokeAsync()
        {
            var userId = User.Identity.GetId();
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == Convert.ToInt32(userId));
            var trades = user?.Trades.Select(x => x.Trade.Id).ToList();
            var query = _repository.Linq<Job>().Include(x => x.Trade).Include(x => x.JobInterests).Where(x => !x.JobInterests.Any(y => y.User == user && !y.IsInterested) && !x.IsFeatured && trades.Contains(x.Trade.Id));

            var notificationRead = _repository.Linq<Notification>().OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.Type == "NEW_JOBS" && x.Read && x.User == user);
            var newJobExists = query.Count(x => x.CreatedAt.Date >= DateTime.Now.Date);
            
            if (notificationRead != null)
            {
                newJobExists = query.Count(x => x.CreatedAt >= notificationRead.ReadAt);
            }

            if (newJobExists > 0)
            {
                _events.Raise(new NotificationCreateEvent(user, "NEW_JOBS"));   
            }

            return Task.FromResult((IViewComponentResult)View("Default", newJobExists));
        }
    }
}