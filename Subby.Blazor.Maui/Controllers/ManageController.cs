using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LastContent.Utilities.Email;
using LastContent.Utilities.GeoCoordinate;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Subby.Core.Interfaces;
using Subby.Blazor.Maui.Extensions;
using Subby.Blazor.Maui.Models.ManageViewModels;

namespace Subby.Blazor.Maui.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly IRepository _repository;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly ISendInBlue _emailSender;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IGeocoder _geocoder;
        private readonly IFileUpload _fileUpload;
        public ManageController(
            IRepository repository,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<ManageController> logger,
            ISendInBlue emailSender,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IGeocoder geocoder,
            IFileUpload fileUpload)
        {
            _repository = repository;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _geocoder = geocoder;
            _fileUpload = fileUpload;
        }


        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userTrades = _repository.Linq<UserTrade>().Where(x => x.User == user).ToList();


            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StatusMessage = StatusMessage,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                IsEmailConfirmed = user.EmailConfirmed,
                MarketingOptIn = user.MarketingOptIn,
                IsTrader = user.IsTrader,
                TradeCity = user.TradeCity,
                TradeDistance = user.TradeDistance,
                TradeName = user.TradeName,
                TradePostcode = user.TradePostcode,
                TradeAddressLine1 = user.TradeAddressLine1,
                TradeAddressLine2 = user.TradeAddressLine2,
                ShowExternalJobs = user.ShowExternalJobs,
                Latitude = user.Latitude != null ? user.Latitude.Value : 0,
                Longitude = user.Longitude != null ? user.Longitude.Value : 0,
                Avatar = user.Avatar
            };



            ViewData["Trades"] = _repository.Linq<Trade>().ToList().OrderBy(x => x.Name).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = userTrades.FirstOrDefault(y => y.Trade.Id == x.Id) != null
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            ViewData["Trades"] = _repository.Linq<Trade>().ToList().OrderBy(x => x.Name).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // delete existing
            var userTrades = _repository.Linq<UserTrade>().Where(x => x.User == user).ToList();
            foreach (var trade in userTrades)
            {
                _repository.Delete(trade);
                _repository.Save();
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = _userManager.SetEmailAsync(user, model.Email).Result;
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = _userManager.SetPhoneNumberAsync(user, model.PhoneNumber).Result;
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            if (model.File != null)
            {
                var uploadResult = _fileUpload.Upload(model.File);
                user.Avatar = uploadResult;
            }

            user.FirstName = model.Firstname;
            user.LastName = model.Lastname;
            user.MarketingOptIn = model.MarketingOptIn;
            user.IsTrader = model.IsTrader;
            user.TradeCity = model.TradeCity;
            user.TradeName = model.TradeName;
            user.TradeDistance = model.TradeDistance;
            user.TradePostcode = model.TradePostcode;
            user.TradeAddressLine1 = model.TradeAddressLine1;
            user.TradeAddressLine2 = model.TradeAddressLine2;
            user.ShowExternalJobs = model.ShowExternalJobs;
            user.Latitude = model.Latitude;
            user.Longitude = model.Longitude;

            // add new trades
            foreach (var trade in model.Trades)
            {
                var selectedTrade = _repository.Linq<Trade>().FirstOrDefault(x => x.Id == trade);
                if (selectedTrade != null)
                {
                    _repository.Add(new UserTrade
                    {
                        Trade = selectedTrade,
                        User = user
                    });
                }
            }

            // locate geo location
            if (user.TradePostcode != model.TradePostcode && !string.IsNullOrEmpty(model.TradePostcode))
            {
                var geoLocation = _geocoder.FindCoordinates(model.TradePostcode, "GB");
                user.Latitude = Convert.ToDouble(geoLocation.Latitude);
                user.Longitude = Convert.ToDouble(geoLocation.Longitude);
            }

            await _userManager.UpdateAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
            var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
            var passwordResetEmailModel = new Models.EmailViewModels.ResetPasswordViewModel
            {
                Name = user.FirstName,
                CallBackUrl = callbackUrl
            };
            var body = _razorViewToStringRenderer
                .RenderViewToStringAsync("Views/Emails/Welcome.cshtml", passwordResetEmailModel).Result;
            _emailSender.Send(model.Email, user.FirstName, "Active your account!", body);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                StatusMessage = "Error: " + changePasswordResult.Errors.FirstOrDefault()?.Description;
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                    + "_"
                    + Guid.NewGuid().ToString().Substring(0, 4)
                    + Path.GetExtension(fileName);
        }
    }
}
