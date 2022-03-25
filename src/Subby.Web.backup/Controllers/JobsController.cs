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
using AutoMapper.QueryableExtensions;
using FluentNHibernate.Utils;
using LastContent.Utilities.Caching;
using LastContent.Utilities.Extensions;
using LastContent.Utilities.GeoCoordinate;
using LastContent.Utilities.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHibernate.Util;
using Subby.Core.Events;
using Subby.Core.Extensions;
using Subby.Core.Models.Job;
using Subby.Core.Services;
using Subby.Infrastructure;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Extensions;
using Subby.Web.ApiModels;
using Subby.Web.Extensions;
using Subby.Web.Models.EmailViewModels;
using Subby.Web.Models.JobViewModels;
using Subby.Web.Models.ReviewViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Subby.Web.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;
        private readonly IMapper _mapper;
        private readonly IGeocoder _geocoder;
        private readonly IFileUpload _fileUpload;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly ILogger _logger;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly IAppCache _cache;
        public JobsController(
            IRepository repository, 
            IDomainEvents events, 
            IMapper mapper,
            IGeocoder geocoder,
            IFileUpload fileUpload,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            ILogger<JobsController> logger,
            IConfigurationProvider mappingConfiguration,
            IAppCache cache
        )
        {
            _repository = repository;
            _events = events;
            _mapper = mapper;
            _geocoder = geocoder;
            _fileUpload = fileUpload;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _logger = logger;
            _mappingConfiguration = mappingConfiguration;
            _cache = cache;

        }
        
        public IActionResult Index(string filters, string keywords, int page = 1)
        {
            
            var results = _cache.AddOrGetExisting($"JOBS-{filters}-{keywords}-{page}", () => GetJobs(filters, keywords, page), DateTimeOffset.Now.AddHours(4));
            ViewData["CanApply"] = CanApplyJobs();
            return View(results);
        }

        private PagedResult<JobModel> GetJobs(string filters, string keywords, int page)
        {
            var query = _repository.Linq<Job>().Where(x => x.CreatedAt >= DateTime.Now.AddMonths(-6));
            var user = _repository.Linq<User>().Include(x => x.Trades).FirstOrDefault(x => x.Email == User.Identity.Name);

            switch (filters)
            {
                case "featured":
                    query = query.Where(x => x.IsFeatured).OrderByDescending(x => x.CreatedAt);
                    break;
                case "popular":
                    query = query.Where(x => x.Activities.Count > 0).OrderByDescending(x => x.Activities.Count);
                    break;
                case "new":
                    var notificationRead = _repository.Linq<Notification>().OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefault(x => x.Type == "NEW_JOBS" && x.Read && x.User == user);
                    var trades = user?.Trades.Select(x => x.Trade.Id).ToList();
                    query = query.Include(x => x.Trade)
                        .Where(x => x.CreatedAt.Date >= DateTime.Now.Date && trades.Contains(x.Trade.Id));
                    if (notificationRead != null)
                    {
                        query = query.Where(x => x.CreatedAt >= notificationRead.ReadAt);
                    }

                    // mark notification as read
                    _events.Raise(new NotificationReadEvent(user, "NEW_JOBS"));
                    break;
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

            // personalize user interests
            query = query
                .Include(x => x.User)
                .Include(x => x.JobInterests).Where(x =>
                    !x.JobInterests.Any(y => y.User == user && !y.IsInterested) && !x.IsFilled &&
                    x.UpdatedAt >= DateTime.Now.AddMonths(-6));

            if (!user.ShowExternalJobs)
            {
                query = query.Where(x => !x.IsExternal);
            }

            var results = query.ProjectTo<JobModel>(_mappingConfiguration);
            return results.ToPagination(page, 20);
        }


        public IActionResult MyJobs(int page = 1)
        {
            var userId = User.Identity.GetId();
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == Convert.ToInt32(userId));
            var query = _repository.Linq<Job>().Include(x => x.Applications).Where(x => x.User == user);
            ViewData["CanApply"] = CanApplyJobs();
            var results = query.ProjectTo<JobModel>(_mappingConfiguration);
            var jobs = results.ToPagination(page, 20);
            
            var applicationNotification = _repository
                .Linq<Notification>().FirstOrDefault(x => x.Type == "NEW_APPLICATIONS" && !x.Read && x.User == user);
            if (applicationNotification != null)
            {
                applicationNotification.Read = true;
                applicationNotification.ReadAt = DateTime.Now;
                _repository.Update(applicationNotification);
            }
            
            return View(jobs);
        }
        
        public IActionResult Details(string slug)
        {
            var job = _repository.Linq<Job>()
                .Include(x => x.Trade)
                .FirstOrDefault(x => x.Slug == slug);
            
            

            if (job == null)
            {
                return NotFound();
            }

            job.IsApplied = job.Applications.Any(x => x.Applicant.Email == User.Identity.Name);
            
            _events.Raise(new JobViewEvent(job));
            
            ViewData["CanApply"] = CanApplyJobs();
            
            ViewData["Review"] = CalculateRating(job);
            
            return View(job);
        }

        private RatingScore CalculateRating(Job job)
        {
            var result = new RatingScore();
            if (job.User == null)
            {
                return result;
            }
            
            var userReviews = _repository.Linq<UserReview>()
                .Where(x => x.User.Id == job.User.Id).ToList();
            
            int courtesyStar5 = 0;
            int courtesyStar4 = 0;
            int courtesyStar3 = 0;
            int courtesyStar2 = 0;
            int courtesyStar1 = 0;
            
            int tidyStar5 = 0;
            int tidyStar4 = 0;
            int tidyStar3 = 0;
            int tidyStar2 = 0;
            int tidyStar1 = 0;
            
            int reliabilityStar5 = 0;
            int reliabilityStar4 = 0;
            int reliabilityStar3 = 0;
            int reliabilityStar2 = 0;
            int reliabilityStar1 = 0;

            foreach (var review in userReviews)
            {
                if (review.Courtesy == 5) courtesyStar5++;
                if (review.Tidiness == 5) tidyStar5++;
                if (review.Reliability == 5) reliabilityStar5++;

                if (review.Courtesy == 4) courtesyStar4++;
                if (review.Tidiness == 4) tidyStar4++;
                if (review.Reliability == 4) reliabilityStar4++;

                if (review.Courtesy == 3) courtesyStar3++;
                if (review.Tidiness == 3) tidyStar3++;
                if (review.Reliability == 3) reliabilityStar3++;

                if (review.Courtesy == 2) courtesyStar2++;
                if (review.Tidiness == 2) tidyStar2++;
                if (review.Reliability == 2) reliabilityStar2++;

                if (review.Courtesy == 1) courtesyStar1++;
                if (review.Tidiness == 1) tidyStar1++;
                if (review.Reliability == 1) reliabilityStar1++;
            }

            result.Courtesy = RatingCalculation.GetRating(courtesyStar1, courtesyStar2, courtesyStar3, courtesyStar4, courtesyStar5);
            result.Tidiness = RatingCalculation.GetRating(tidyStar1, tidyStar2, tidyStar3, tidyStar4, tidyStar5);
            result.Reliability = RatingCalculation.GetRating(reliabilityStar1, reliabilityStar2, reliabilityStar3, reliabilityStar4, reliabilityStar5);
            
            result.TotalReviews = userReviews.Count;
            return result;
        }

        private bool CanApplyJobs()
        {
            var user = _repository.Linq<User>()
                .Include(x => x.Subscriptions)
                .Include(x => x.Applications)
                .FirstOrDefault(x => x.Email == User.Identity.Name);
            var subscriptionExpiryDate =
                user.Subscriptions.OrderByDescending(x => x.ExpiryDate).FirstOrDefault();
            var activeSubscription = subscriptionExpiryDate != null && subscriptionExpiryDate.ExpiryDate >= DateTime.Now;

            var canApply = activeSubscription;

            var freeApplication = _repository.Linq<Configuration>().GetOrDefault<int>("FREE_APPLICATION", 0);

            if (freeApplication > 0)
            {
                if (user.Applications.Count < freeApplication)
                {
                    canApply = true;
                }
            }

            return canApply;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<NewJobViewModel>(job);
            SelectListTrades();
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Edit(NewJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SelectListTrades();

                return View();
            }

            var job = _mapper.Map<Job>(model);
            job.UpdatedAt = DateTime.Now;

            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var trade = _repository.Linq<Trade>().FirstOrDefault(x => x.Id == model.TradeId);
            job.User = user;
            job.Slug = model.Title.ToSlug();
            job.Trade = trade;
            
            // locate geo location
            if (!string.IsNullOrEmpty(model.Postcode))
            {
                try
                {
                    var geoLocation = _geocoder.FindCoordinates(model.Postcode, "GB");
                    job.Latitude = Convert.ToDouble(geoLocation.Latitude);
                    job.Longitude = Convert.ToDouble(geoLocation.Longitude);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to store user location");
                }
            }

            if (!string.IsNullOrEmpty(job.ExternalLink))
            {
                job.IsExternal = true;
            }
            
            try
            {
                if (model.File != null)
                {
                    var uploadResult = _fileUpload.Upload(model.File);
                    job.Media = uploadResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload image file for job {job.Title}");
            }
            
            _repository.Update(job);
            
            return RedirectToAction(nameof(AddComplete));
        }
        
        [HttpGet]
        public IActionResult Apply(int id)
        {
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            var model = new ApplyJobViewModel
            {
                Job = job,
                JobId = job.Id
            };
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Apply(ApplyJobViewModel model)
        {
            var job = _repository.Linq<Job>().Include(x => x.User).FirstOrDefault(x => x.Id == model.JobId);
            
            if (!ModelState.IsValid)
            {
                var jobModel = new ApplyJobViewModel
                {
                    Job = job,
                    JobId = job.Id
                };
                return View(jobModel);
            }

            var file = "";
            if (model.File != null)
            {
                var uploadResult = _fileUpload.Upload(model.File);
                file = uploadResult;
            }

            var application = new JobApplication
            {
                EligibleToWorkInEu = model.EligibleToWorkInEu,
                EligibleToWorkInUk = model.EligibleToWorkInUk,
                Applicant = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name),
                Job = job,
                File = file,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Phone = model.Phone,
                Email = model.Email,
                Quote = model.Quote,
                Estimation = model.Estimation
            };
            
            // send the email to the user
            
            var callbackUrl = Request.Scheme + "://" + Request.Host + "/applications/index/" + job.Id;
            // var callbackUrl = Url.JobApplicationLink(job.Id, Request.Scheme);
            var emailModel = new NewApplicationModel
            {
                ApplicationName = model.Firstname + " " + model.Lastname,
                JobTitle = job.Title,
                User = job.User.FirstName,
                CallBackUrl = callbackUrl
            };

            var body = _razorViewToStringRenderer
                .RenderViewToStringAsync("Views/Emails/NewApplication.cshtml", emailModel).Result;
                
            _events.Raise(new EmailEvent(job.User.Email, body, "New Application", job.User.FirstName ));
            _events.Raise(new NewApplicationEvent(job.User));
            _repository.Add(application);

            return RedirectToAction(nameof(ApplyComplete));
        }

        public IActionResult ApplyComplete()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult New()
        {
            SelectListTrades();
            return View();
        }
        
        [HttpPost]
        public IActionResult New(NewJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SelectListTrades();

                return View(model);
            }

            var job = _mapper.Map<Job>(model);
            job.CreatedAt = DateTime.Now;
            job.UpdatedAt = DateTime.Now;

            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var trade = _repository.Linq<Trade>().FirstOrDefault(x => x.Id == model.TradeId);
            job.User = user;
            job.Slug = model.Title.ToSlug();
            job.Trade = trade;
            
            // locate geo location
            if (!string.IsNullOrEmpty(model.Postcode))
            {
                try
                {
                    var geoLocation = _geocoder.FindCoordinates(model.Postcode, "GB");
                    job.Latitude = Convert.ToDouble(geoLocation.Latitude);
                    job.Longitude = Convert.ToDouble(geoLocation.Longitude);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to store user location");
                }
            }

            if (!string.IsNullOrEmpty(job.ExternalLink))
            {
                job.IsExternal = true;
            }

            try
            {
                if (model.File != null)
                {
                    var uploadResult = _fileUpload.Upload(model.File);
                    job.Media = uploadResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload image file for job {job.Title}");
            }
            
            _repository.Add(job);

            job.Slug = job.Slug + "-" + job.Id;
            _repository.Update(job);
            
            return RedirectToAction(nameof(AddComplete));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult IsInterested(int id)
        {
            if (id <= 0) return new ForbidResult();
            
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == id);
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            _events.Raise(new JobInterestEvent(job, true, user));
            
            return new OkResult();
        }
        
        [HttpGet("[action]")]
        public IActionResult AddComplete()
        {
            return View();
        }
        
        [HttpGet("[action]/{id}/{jobId}")]
        public IActionResult Review(int id, int jobId)
        {
            var model = new WriteAReviewViewModel
            {
                UserId = id,
                JobId = jobId,
                DateCompleted = DateTime.Now
            };
            return View(model);
        }
        
        [HttpPost("[action]/{id}/{jobId}")]
        [ValidateAntiForgeryToken]
        public IActionResult Review(WriteAReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == model.UserId);
            var job = _repository.Linq<Job>().FirstOrDefault(x => x.Id == model.JobId);
            var reviewer = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var review = new UserReview
            {
                Job = job,
                User = user,
                Reviewer = reviewer,
                Review = model.Review,
                Reliability = model.Reliability,
                Courtesy = model.Courtesy,
                Tidiness = model.Tidiness,
                DateCompleted = model.DateCompleted,
                DateCreated = DateTime.Now
            };
            
            _repository.Add(review);
            
            return RedirectToAction("ReviewComplete", "Jobs", new { id = model.JobId });
        }
        
        [HttpGet("[action]")]
        public IActionResult ReviewComplete(int id)
        {
            return View();
        }

        private void SelectListTrades()
        {
            ViewData["Trades"] = _repository.Linq<Trade>().Where(x => x.Active).ToList().OrderBy(x => x.Name).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            var contractTypes = new List<string>()
            {
                "Full Time",  "Part Time", "Temporary", "Contract"
            };
            
            ViewData["ContractTypes"] = contractTypes.ToList().Select(x => new SelectListItem()
            {
                Text = x,
                Value = x
            });
        }
    }
}
