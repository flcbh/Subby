using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using AutoMapper;
using LastContent.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;
using Subby.Web.New.Models.ConfigurationViewModels;

namespace Subby.Web.New.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SubscriptionsController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public SubscriptionsController(
            IRepository repository,
            ILogger<SubscriptionsController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _repository.Linq<Subscription>().Include(x => x.User).Where(x => x.ExpiryDate.Date >= DateTime.Now.Date && x.User.Role != "Administrator");
            var trades = query.ToPagination(page, 20);
            return View(trades);
        }
    }
}
