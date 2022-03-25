using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using AutoMapper;
using LastContent.Utilities.Extensions;
using LastContent.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace Subby.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdvertCategoriesController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IHostEnvironment _environment;
        private readonly IMapper _mapper;
        public AdvertCategoriesController(
            IRepository repository,
            ILogger<AdvertCategoriesController> logger,
            IHostEnvironment environment,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _environment = environment;
            _mapper = mapper;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _repository.Linq<AdvertCategory>();
            var trades = query.ToPagination(page, 20);
            return View(trades);
        }
        
        
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Add(AdvertCategory model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Slug = model.Name.ToSlug();
            _repository.Add(model);
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var trade = _repository.Linq<AdvertCategory>().FirstOrDefault(x => x.Id == id);
            return View(trade);
        }
        

        [HttpPost]
        public IActionResult Edit(AdvertCategory model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            model.Slug = model.Name.ToSlug();
            _repository.Update(model);
  
            return RedirectToAction(nameof(Index));
        }

    }
}
