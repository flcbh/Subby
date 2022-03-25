using System;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LastContent.Utilities.Email;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Blazor.Maui.Models.EmailViewModels;
using Subby.Blazor.Maui.ApiModels;
using Subby.Blazor.Maui.Extensions;

namespace Subby.Blazor.Maui.Api
{
    public class ApplicationsController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IDomainEvents _events;
        public ApplicationsController(
            IRepository repository,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IDomainEvents events)
        {
            _repository = repository;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _events = events;
        }

        [HttpPost]
        public IActionResult Reject([FromBody] RejectRequest model)
        {
            var item = _repository.Linq<JobApplication>()
                .Include(x => x.Applicant)
                .Include(x => x.Job)
                .FirstOrDefault(x => x.Id == model.Id);
            if (item != null)
            {
                item.Accepted = false;
                item.UpdatedAt = DateTime.Now;
                item.Rejected = true;
                item.Read = true;
                _repository.Update(item);

                var emailModel = new NotSuitableViewModel
                {
                    Name = item.Firstname,
                    JobTitle = item.Job.Title
                };

                var body = _razorViewToStringRenderer
                    .RenderViewToStringAsync("Views/Emails/NotSuitable.cshtml", emailModel).Result;

                _events.Raise(new ApplicationNotSuitableEvent(item.Email, body, item.Firstname));

            }

            return Ok();
        }
    }
}
