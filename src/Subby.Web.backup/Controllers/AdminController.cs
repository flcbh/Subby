using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Subby.Web.Models.AdminViewModels;

namespace Subby.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IHostEnvironment _environment;
        private readonly IMapper _mapper;
        public AdminController(
            IRepository repository,
            ILogger<AdminController> logger,
            IHostEnvironment environment,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _environment = environment;
            _mapper = mapper;
        }

        public IActionResult Index()
        {

            var model = new DashboardViewModel();
            var totalSubscriptions = _repository.Linq<Subscription>().Include(x => x.User).Count(x => x.ExpiryDate.Date >= DateTime.Now.Date && x.User.Role != "Administrator");
            var totalRevenue = _repository.Linq<Subscription>().Include(x => x.User).Where(x => x.ExpiryDate.Date >= DateTime.Now.Date && x.User.Role != "Administrator").Sum(x => x.Amount);
            var totalJobs = _repository.Linq<Job>().Count();
            var totalNewJobs = _repository.Linq<Job>().Count(x => x.CreatedAt.Date == DateTime.Now.Date);
            
            model.Revenue = totalRevenue > 0 ? totalRevenue : 0;
            model.CountJobs = totalJobs;
            model.CountNewJobs = totalNewJobs;
            model.CountSubscriptions = totalSubscriptions;
            model.Users = _repository.Linq<User>().ToList();
            model.Sponsors = _repository.Linq<Sponsor>().Count();
            model.CountTrades = _repository.Linq<Trade>().Count();
            model.Configurations = _repository.Linq<Configuration>().Count();
            model.Benefits = _repository.Linq<Benefit>().Count();
            model.Adverts = _repository.Linq<Advert>().Count();
            model.AdvertCategories = _repository.Linq<AdvertCategory>().Count();

            return View(model);
        }
        #region helpers
        
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return  Path.GetFileNameWithoutExtension(fileName)
                    + "_" 
                    + Guid.NewGuid().ToString().Substring(0, 4) 
                    + Path.GetExtension(fileName);
        }

        #endregion
        
        
    }
}
