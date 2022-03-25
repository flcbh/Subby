using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using AutoMapper;
using LastContent.Utilities.Pagination;
using Subby.Web.Models.ConfigurationViewModels;

namespace Subby.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ConfigurationsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public ConfigurationsController(
            IRepository repository,
            ILogger<ConfigurationsController> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _repository.Linq<Configuration>();
            var trades = query.ToPagination(page, 20);
            return View(trades);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = _repository.Linq<Configuration>().FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return NotFound();
            }
            
            var model = new ConfigurationViewModel
            {
                Value = data.Value,
                Id = data.Id
            };
            return View(model);
        }
        

        [HttpPost]
        public IActionResult Edit(ConfigurationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var data = _repository.Linq<Configuration>().FirstOrDefault(x => x.Id == model.Id);
            if (data != null)
            {
                data.Value = model.Value;
                data.ModifiedDate = DateTime.Now;
                _repository.Update(data);   
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
