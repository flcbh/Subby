using System;
using System.Collections.Generic;
using System.IO;
using Subby.Core;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastContent.Utilities.Extensions;
using LastContent.Utilities.GeoCoordinate;
using LastContent.Utilities.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Events;
using Subby.Core.Models.Job;
using Subby.Infrastructure;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Extensions;
using Subby.ApiModels;

namespace Subby.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;

        public ApplicationsController(
            IRepository repository,
            IDomainEvents events)
        {
            _repository = repository;
            _events = events;
        }

        public IActionResult Index(int id, string filters, int page = 1)
        {
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == id && x.User == user);

            if (job == null)
            {
                return NotFound();
            }

            var query = _repository.Linq<JobApplication>().Where(x => x.Job == job);

            if (filters == "rejected")
            {
                query = query.Where(x => x.Rejected && x.Read);
            }
            else
            {
                query = query.Where(x => !x.Rejected);
            }

            ViewData["Job"] = job;

            var applications = query.OrderByDescending(x => x.CreatedAt).ToPagination(page, 20);
            return View(applications);
        }
    }
}
