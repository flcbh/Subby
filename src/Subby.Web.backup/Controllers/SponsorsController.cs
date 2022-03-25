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
using Subby.Web.Extensions;
using Subby.Web.Models.AdminViewModels;

namespace Subby.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SponsorsController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IFileUpload _fileUpload;
        public SponsorsController(
            IRepository repository,
            ILogger<SponsorsController> logger,
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
            var query = _repository.Linq<Sponsor>();
            var sponsors = query.ToPagination(page, 20);
            return View(sponsors);
        }
        
        
        [HttpGet]
        public IActionResult Add()
        {
            GetBenefits(null);
            return View();
        }
        
        [HttpPost]
        public IActionResult Add(SponsorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sponsor = _mapper.Map<Sponsor>(model);
            sponsor.CreatedAt = DateTime.Now;
            sponsor.LiveDate = DateTime.Now;

            if (model.File != null)
            {
                var uploadResult = _fileUpload.Upload(model.File);
                sponsor.Logo = uploadResult;
            }

            _repository.Add(sponsor);
            
            foreach (var item in model.Benefits)
            {
                var selectedBenefit = _repository.Linq<Benefit>().FirstOrDefault(x => x.Id == item);
                if (selectedBenefit != null)
                {
                    _repository.Add(new BenefitSponsor
                    {
                        Benefit = selectedBenefit,
                        Sponsor = sponsor
                    });
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var sponsor = _repository.Linq<Sponsor>().FirstOrDefault(x => x.Id == id);
            var model = _mapper.Map<SponsorViewModel>(sponsor);
            GetBenefits(sponsor);
            return View(model);
        }
        

        [HttpPost]
        public IActionResult Edit(SponsorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sponsor = _mapper.Map<Sponsor>(model);
            
            if (model.IsActive)
            {
                sponsor.LiveDate = DateTime.Now;    
            }
            
            if (model.File != null)
            {
                var uploadResult = _fileUpload.Upload(model.File);
                sponsor.Logo = uploadResult;
            }

            _repository.Update(sponsor);
            _repository.Save();
            // delete existing
            var benefits = _repository.Linq<BenefitSponsor>().Where(x => x.Sponsor == sponsor).ToList();
            foreach (var item in benefits)
            {
                _repository.Delete(item);
                _repository.Save();
            }
            
            foreach (var item in model.Benefits)
            {
                var selectedBenefit = _repository.Linq<Benefit>().FirstOrDefault(x => x.Id == item);
                if (selectedBenefit != null)
                {
                    _repository.Add(new BenefitSponsor
                    {
                        Benefit = selectedBenefit,
                        Sponsor = sponsor
                    });
                }
            }
            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        #region helpers
        
        
        private void GetBenefits(Sponsor sponsor)
        {
            if (sponsor != null)
            {
                var selectedBenefits = _repository.Linq<BenefitSponsor>().Where(x => x.Sponsor == sponsor).ToList();
                ViewData["Benefits"] = _repository.Linq<Benefit>().ToList().OrderBy(x => x.Title).Select(x => new SelectListItem()
                {
                    Text = x.Title,
                    Value = x.Id.ToString(),
                    Selected = selectedBenefits.FirstOrDefault(y => y.Benefit.Id == x.Id) != null
                });
            }
            else
            {
                ViewData["Benefits"] = _repository.Linq<Benefit>().ToList().OrderBy(x => x.Title).Select(x => new SelectListItem()
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                });
            }
            
        }
        
        [TempData]
        public string StatusMessage { get; set; }

        #endregion
        
        
    }
}
