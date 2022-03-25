using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LastContent.Utilities.Extensions;
using LastContent.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Subby.Blazor.Maui.Extensions;
using Subby.Blazor.Maui.Models.AdminViewModels;

namespace Subby.Blazor.Maui.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManageBenefits : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        public ManageBenefits(
            IRepository repository,
            ILogger<ManageBenefits> logger,
            IMapper mapper,
            IFileUpload fileUpload)
        {
            _repository = repository;
            _logger = logger;
            _fileUpload = fileUpload;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            var query = _repository.Linq<Benefit>();
            var sponsors = query.ToPagination(page, 20);
            return View(sponsors);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(BenefitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var benefit = _mapper.Map<Benefit>(model);
            benefit.Slug = model.Title.ToSlug();

            _repository.Add(benefit);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var sponsor = _repository.Linq<Benefit>().FirstOrDefault(x => x.Id == id);
            var model = _mapper.Map<BenefitViewModel>(sponsor);
            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(BenefitViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var benefit = _mapper.Map<Benefit>(model);
            benefit.Slug = model.Title.ToSlug();

            _repository.Update(benefit);
            return RedirectToAction(nameof(Index));
        }

        #region helpers

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                    + "_"
                    + Guid.NewGuid().ToString().Substring(0, 4)
                    + Path.GetExtension(fileName);
        }

        private void UserRoles()
        {
            ViewData["Roles"] = _repository.Linq<Role>().ToList().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Name
            });
        }


        #endregion


    }
}
