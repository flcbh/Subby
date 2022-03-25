using System;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Subby.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Linq;
using AutoMapper;
using LastContent.Utilities.Notification;
using LastContent.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;
using Notification = FirebaseAdmin.Messaging.Notification;
using Subby.Web.New.Models.FIrebaseViewModels;
using Subby.Web.New.Models.AdminViewModels;

namespace Subby.Web.New.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        private readonly IHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly INotificationService _notification;
        public UsersController(
            IRepository repository,
            ILogger<UsersController> logger,
            IHostEnvironment environment,
            IMapper mapper,
            INotificationService notification)
        {
            _repository = repository;
            _logger = logger;
            _environment = environment;
            _mapper = mapper;
            _notification = notification;
        }

        public IActionResult Index(string acceptPush, int page = 1)
        {
            var query = _repository.Linq<User>();

            if (!string.IsNullOrEmpty(acceptPush))
            {
                query = query.Where(x => !x.PushToken.Equals(string.Empty));
            }

            query = query.Include(x => x.Subscriptions);

            var users = query.ToPagination(page, 20);
            return View(users);
        }

        // [HttpGet("[action]")]
        // public IActionResult Profile()
        // {
        //     return PartialView();
        // }
        //
        // [HttpPost("[action]")]
        // public IActionResult Profile(string test)
        // {
        //     Notification.Set("Test", "Test Notification", NotificationType.Success);
        //     return PartialView();
        // }

        [HttpGet]
        public IActionResult Push(int id)
        {
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == id);
            var model = new PushViewModel
            {
                UserId = id,
                Name = user.FirstName + " " + user.LastName
            };
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Push(PushViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }



            if (model.UserId > 0)
            {
                var user = _repository.Linq<User>().Include(x => x.Tokens).FirstOrDefault(x => x.Id == model.UserId);

                SendMessage(model, user);
            }
            else
            {
                var users = _repository.Linq<User>().Include(x => x.Tokens).Where(x => !x.PushToken.Equals(string.Empty)).ToList();
                foreach (var user in users)
                {
                    SendMessage(model, user);
                }
            }
            _notification.Set("Push Notification", "Message sent successfully", NotificationType.Success);
            return PartialView(model);
        }

        private void SendMessage(PushViewModel model, User user)
        {
            try
            {
                if (user.Tokens.Any())
                {
                    foreach (var token in user.Tokens)
                    {
                        var message = new NewJobViewModel
                        {
                            CollapseKey = "New Message",
                            Priority = "high",
                            To = token.Token,
                            Notification = new Subby.Web.New.Models.FIrebaseViewModels.Notification
                            {
                                Title = $"Hi {user.FirstName}",
                                Body = model.Message
                            }
                        };

                        var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                        var request = new RestRequest("", Method.Post);
                        request.AddHeader("Authorization", "key=AAAAmmNT_wQ:APA91bHEe3rCs0EF2KD6XhP2jGHj01q4U3bcFu9aOggnxcX-mKSNNaRKQ-W8flECC3bNgzEdA69ke6J6KY1oY1ybFF8V0DPg7pabYa99N3PD7FNbQoSo4rstIld0AvseqEPVwFlVowl9AGrTPQQoX-ywTHWlS9Qk2A");
                        request.AddHeader("Content-Type", "application/json");
                        request.AddJsonBody(JsonConvert.SerializeObject(message));
                        var response = client.PostAsync(request);

                        _logger.LogInformation($"Push notification sent for {user.FirstName} status: {response.Result.Content}");
                    }
                }
                else
                {
                    var message = new NewJobViewModel
                    {
                        CollapseKey = "New Message",
                        Priority = "high",
                        To = user.PushToken,
                        Notification = new Subby.Web.New.Models.FIrebaseViewModels.Notification
                        {
                            Title = $"Hi {user.FirstName}",
                            Body = model.Message
                        }
                    };

                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest("", Method.Post);
                    request.AddHeader("Authorization", "key=AAAAmmNT_wQ:APA91bHEe3rCs0EF2KD6XhP2jGHj01q4U3bcFu9aOggnxcX-mKSNNaRKQ-W8flECC3bNgzEdA69ke6J6KY1oY1ybFF8V0DPg7pabYa99N3PD7FNbQoSo4rstIld0AvseqEPVwFlVowl9AGrTPQQoX-ywTHWlS9Qk2A");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody(JsonConvert.SerializeObject(message));
                    var response = client.PostAsync(request);

                    _logger.LogInformation($"Push notification sent for {user.FirstName} status: {response.Result.Content}");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Push notification failed");
            }

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserRoles();

            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == id);
            var model = _mapper.Map<UserViewModel>(user);
            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            UserRoles();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _repository.Linq<User>().FirstOrDefault(x => x.Id == model.Id);

            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Role = model.Role;

                _repository.Update(user);
            }

            return RedirectToAction(nameof(Index));
        }

        #region helpers

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
