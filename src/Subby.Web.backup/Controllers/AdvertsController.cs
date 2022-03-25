using System;
using System.Collections.Generic;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using LastContent.Utilities.Caching;
using LastContent.Utilities.Extensions;
using LastContent.Utilities.GeoCoordinate;
using LastContent.Utilities.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Subby.Utilities.DomainEvents;
using Subby.Web.Extensions;
using Subby.Web.Models.AdvertViewModels;

namespace Subby.Web.Controllers
{
    [Authorize]
    public class AdvertsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;
        private readonly IMapper _mapper;
        private readonly IGeocoder _geocoder;
        private readonly IFileUpload _fileUpload;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly ILogger _logger;
        private readonly IAppCache _cache;
        private readonly List<string> _cacheKeys = new List<string>();
        public AdvertsController(
            IRepository repository, 
            IDomainEvents events, 
            IMapper mapper,
            IGeocoder geocoder,
            IFileUpload fileUpload,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            ILogger<AdvertsController> logger,
            IAppCache cache)
        {
            _repository = repository;
            _events = events;
            _mapper = mapper;
            _geocoder = geocoder;
            _fileUpload = fileUpload;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _logger = logger;
            _cache = cache;
        }
        
        public IActionResult Index(string filters, string keywords, int categoryId, string isFree, string isSold, int miles, int page = 1)
        {
            var filter = $"ADVERTS-{filters}-{keywords}-{categoryId}-{isFree}-{isSold}-{miles}-{page}";
            this._cacheKeys.Add(filter);
            //var model = _cache.AddOrGetExisting(
            //  filter, 
            // () => GetAdverts(filters, keywords, categoryId, isFree, isSold, page), 
            //DateTimeOffset.Now.AddMinutes(10));
            
            var model = GetAdverts(filters, keywords, categoryId, isFree, isSold, miles, page);

            var currentUser = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);

            var chatQuery = _repository.Linq<ChatChannel>().Where(x => x.MemberCollection.Any(y => y.User == currentUser && !y.IsLeft)).ToList();
            
            var count = 0;
            chatQuery.ForEach(x => { 
                count += x.ChatCollection.Where(last => {
                    return last.User.Id != currentUser.Id && !last.isread;
                }).Count();
             });
            model.messages = count;
            return View(model);
        }

        private AdvertIndexViewModel<Advert> GetAdverts(string filters, string keywords, int categoryId, string isFree, string isSold,
            int miles, int page)
        {
            var query = _repository.Linq<Advert>();


            
            switch (filters)
            {
                default:
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;
            }

            if (!string.IsNullOrEmpty(keywords))
            {
                var searches = keywords.Split(new char[] {' '});

                foreach (var search in searches)
                {
                    query = query.Where(x => x.Title.Contains(search)
                                             || x.Postcode.Contains(search)
                                             || x.Location.Contains(search));
                }
            }

            if (miles > 0)
            {
                var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
                var postcode = user.TradePostcode;
                var longitude = Convert.ToDouble(user.Longitude);
                var latitude = Convert.ToDouble(user.Latitude);

                if (Convert.ToInt32(longitude) != 0 && Convert.ToInt32(latitude) != 0)
                {
                    query = query.Where(x =>
                        3958.75 * Math.Acos(Math.Sin(((double)latitude / 57.2958)) *
                        Math.Sin((double)x.Latitude / 57.2958) +
                        Math.Cos((double)latitude / 57.2958) *
                        Math.Cos((double)x.Latitude / 57.2958) *
                        Math.Cos((double)x.Longitude / 57.2958 - (double)longitude / 57.2958)) <= miles
                    );
                } else
                {
                    query = Enumerable.Empty<Advert>().AsQueryable();
                }
            }

            if (categoryId > 0)
            {
                query = query.Where(x => x.Category.Id == categoryId);
            }

            if (isFree == "on")
            {
                query = query.Where(x => x.IsFree);
            }

            if (isSold == "on")
            {
                query = query.Where(x => x.IsSold);
            }
            /*else
            {
                query = query.Where(x => !x.IsSold);
            }*/

            //query = query.Where(x => x.UpdatedAt >= DateTime.Now.AddMonths(-6));
            var adverts = query.ToPagination(page, 20);


            var model = new AdvertIndexViewModel<Advert>
            {
                Items = adverts,
                Categories = _repository.Linq<AdvertCategory>().Where(x => x.Active).ToList().OrderBy(x => x.Name).ToList(),
                CategoryId = categoryId,
                IsFree = isFree == "on",
                IsSold = isSold == "on",
                miles = miles > 0 ? miles : -1,
                messages = 0,
            };
            return model;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var advert = _repository.Linq<Advert>().FirstOrDefault(x => x.Id == id);
            return View(advert);
        }

        [HttpGet]
        public IActionResult New()
        {
            SelectListCategories();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(NewAdvertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SelectListCategories();
                return View(model);
            }

            var advert = _mapper.Map<Advert>(model);

            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var category = _repository.Linq<AdvertCategory>().FirstOrDefault(x => x.Id == model.CategoryId);
            advert.User = user;
            advert.Slug = model.Title.ToSlug();
            advert.Category = category;
            
            // locate geo location
            if (!string.IsNullOrEmpty(model.Postcode))
            {
                try
                {
                    var geoLocation = _geocoder.FindCoordinates(model.Postcode, "GB");
                    advert.Latitude = Convert.ToDouble(geoLocation.Latitude);
                    advert.Longitude = Convert.ToDouble(geoLocation.Longitude);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to store user location");
                }
            }

            var files = new List<Media>();

            try
            {
                if (model.Files != null)
                {
                    foreach (var file in model.Files)
                    {
                        var uploadResult = _fileUpload.Upload(file);    
                        files.Add(new Media
                        {
                            Src = uploadResult
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload image file for advert {advert.Title}");
            }
            _repository.Add(advert);
            advert.Slug = advert.Slug + "-" + advert.Id;
            _repository.Update(advert);

            // add media collection
            foreach (var item in files)
            {
                item.Advert = advert;
                _repository.Add(item);
            }
            
            return RedirectToAction(nameof(AddComplete));
        }
        
        [HttpGet]
        public IActionResult AddComplete()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Manage(int page)
        {
            var currentUser = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            this._cacheKeys.ForEach(x =>
            {
                _cache.Clear(x);
            });
            var query = _repository.Linq<Advert>().Where(x => x.User == currentUser);
            return View(query.ToPagination(page, 20));
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var advert = _repository.Linq<Advert>().FirstOrDefault(x => x.Id == id);

            if (advert == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<NewAdvertViewModel>(advert);
            SelectListCategories();
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(NewAdvertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SelectListCategories();

                return View();
            }

            var advert = _mapper.Map<Advert>(model);
            advert.UpdatedAt = DateTime.Now;
            advert.MediaCollection = _repository.Linq<Media>().Where(x => x.Advert.Id == model.Id).ToList();
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var category = _repository.Linq<AdvertCategory>().FirstOrDefault(x => x.Id == model.CategoryId);
            advert.User = user;
            advert.Slug = model.Title.ToSlug();
            advert.Category = category;
            
            // locate geo location
            if (!string.IsNullOrEmpty(model.Postcode))
            {
                try
                {
                    var geoLocation = _geocoder.FindCoordinates(model.Postcode, "GB");
                    advert.Latitude = Convert.ToDouble(geoLocation.Latitude);
                    advert.Longitude = Convert.ToDouble(geoLocation.Longitude);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to store user location");
                }
            }
            
            var files = new List<Media>();
            
            try
            {
                if (model.Files != null)
                {
                    foreach (var file in model.Files)
                    {
                        var uploadResult = _fileUpload.Upload(file);

                        files.Add(new Media
                        {
                            Src = uploadResult
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload image file for job {advert.Title}");
            }
            
            _repository.Update(advert);
            
            // add media collection
            foreach (var item in files)
            {
                item.Advert = advert;
                _repository.Add(item);
            }

            return RedirectToAction(nameof(AddComplete));
        }
        
        private void SelectListCategories()
        {
            ViewData["Categories"] = _repository.Linq<AdvertCategory>().Where(x => x.Active).ToList().OrderBy(x => x.Name).Select(
                x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
        }
    }
}
