
using System;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Web.New.ApiModels;

namespace Subby.Web.New.Api
{
    public class AdvertsController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;

        public AdvertsController(IRepository repository, IDomainEvents events)
        {
            _repository = repository;
            _events = events;
        }

        [HttpPost]
        public IActionResult Reject([FromBody] RejectRequest model)
        {
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == model.Id);

            if (job != null)
            {
                var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
                _events.Raise(new JobInterestEvent(job, false, user));
            }

            return Ok();
        }


        [HttpPost("[action]")]
        public IActionResult Filled([FromBody] RejectRequest model)
        {
            var advert = _repository.Linq<Advert>().FirstOrDefault(x => x.Id == model.Id);

            if (advert != null)
            {
                advert.IsSold = true;
                _repository.Update(advert);
            }

            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult OnApply([FromBody] RejectRequest model)
        {
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == model.Id);

            if (job == null)
            {
                return BadRequest("Job not found");
            }

            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var application = new JobApplication
            {
                Applicant = user,
                Job = job,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Phone = user.PhoneNumber,
                Email = user.Email
            };

            _repository.Add(application);

            return Ok();
        }
    }
}
