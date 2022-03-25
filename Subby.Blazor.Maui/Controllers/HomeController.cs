using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Subby.Core.Entities;
using Subby.Blazor.Maui.Models.HomeViewModels;
using Subby.Blazor.Maui.Extensions;

namespace Subby.Blazor.Maui.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        public HomeController(
            IRepository repository,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IHttpContextAccessor context,
            ILogger<HomeController> logger)
        {
            _repository = repository;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var model = new HomeIndexViewModel
            {
                TotalJobs = _repository.Linq<Job>().Count(x => !x.IsFilled && x.UpdatedAt >= DateTime.Now.AddMonths(-6)),
                TotalUsers = _repository.Linq<User>().Count()
            };
            //            return RedirectToAction("Index","Jobs");
            return View(model);
        }

        [HttpGet("/landing")]

        public IActionResult Landing()
        {
            return View();
        }

        [HttpGet("/disclaimer")]

        public IActionResult Disclaimer()
        {
            return View();
        }

        public IActionResult Job()
        {
            //            return RedirectToAction("Index","Jobs");
            return View();
        }

        [HttpGet("/error")]

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Email()
        {

            var passwordResetEmailModel = new Models.EmailViewModels.ResetPasswordViewModel
            {
                Name = "Tee",
                CallBackUrl = "http://localhost:57679/home/email"
            };
            var body = _razorViewToStringRenderer
                .RenderViewToStringAsync("Views/Emails/Welcome.cshtml", passwordResetEmailModel).Result;

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = body
            };
        }


        [HttpGet("get-started")]
        public IActionResult GetStarted()
        {
            var options = new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) };
            Response.Cookies.Append("NEW_USER", "false", options);
            return View();
        }
    }
}
