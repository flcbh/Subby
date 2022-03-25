using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Subby.Utilities.Extensions;
using Subby.Utilities.Interfaces;
using Notification = Subby.Core.Entities.Notification;

namespace Subby.ViewComponents
{
    public class NewApplicationViewComponent : ViewComponent
    {
        private readonly IRepository _repository;

        public NewApplicationViewComponent(IRepository repository)
        {
            _repository = repository;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var userId = User.Identity.GetId();
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == Convert.ToInt32(userId));

            var notifications = _repository.Linq<Notification>().Where(x => x.Type == "NEW_APPLICATIONS" && !x.Read && x.User == user).ToList();
            var applicationCount = 0;

            if (notifications.Any())
            {
                applicationCount = notifications.Sum(x => Convert.ToInt32(x.Value));
            }

            return Task.FromResult((IViewComponentResult)View("Default", applicationCount));
        }
    }
}