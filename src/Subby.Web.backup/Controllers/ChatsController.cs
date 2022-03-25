using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Subby.Core.Models.Job;
using Subby.Utilities.Extensions;
using Subby.Web.Models;
using Subby.Web.Models.ChatViewModels;
using Subby.Web.Models.FIrebaseViewModels;

namespace Subby.Web.Controllers
{
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;
        public ChatsController(IRepository repository, ILogger<ChatsController> logger
            )
        {
            _repository = repository;
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            var currentUser = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var query = _repository.Linq<ChatChannel>().Where(x => x.MemberCollection.Any(y => y.User == currentUser && !y.IsLeft)).ToList()
                .OrderByDescending(x => x.ChatCollection.LastOrDefault().CreatedAt);
            return View(query);
        }

        [HttpGet]
        public IActionResult Channel(long id)
        {
            var currentUser = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var query = _repository.Linq<ChatChannel>().FirstOrDefault(x => x.MemberCollection.Any(y => y.User == currentUser && !y.IsLeft) && x.Id == id);
            
            if (query == null)
            {
                return NotFound();
            }

            var hasUnreadMessage = query.ChatCollection.LastOrDefault();

            if (!hasUnreadMessage.isread && currentUser.Id != hasUnreadMessage.User.Id)
            {
                foreach (var item in query.ChatCollection)
                {
                    this.Filled(item);
                }
            }
            
            return View(query);
            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(ChatMessage model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            var channel = _repository.Linq<ChatChannel>().FirstOrDefault(x => x.Id == model.ChannelId);

            if (channel != null)
            {
                var newMessage = new Chat
                {
                    User = currentUser,
                    Message = model.Message,
                    Channel = channel
                };

                _repository.Add(newMessage);

                foreach (var item in channel.MemberCollection.Where(x => x.User != currentUser).ToList())
                {
                    PushMessage(item.User, model.Message, item.Channel.Id);
                }
            }

            return RedirectToAction(nameof(Channel), new { id = model.ChannelId });
        }
        
        [HttpGet]
        public IActionResult Message(int id)
        {

            var advert = _repository.Linq<Advert>().FirstOrDefault(x => x.Id == id);
            
            var model = new ChatStart
            {
                AdvertId = id,
                Advert = advert
            };
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Message(ChatStart model)
        {
            var advert = _repository.Linq<Advert>().FirstOrDefault(x => x.Id == model.AdvertId);
            model.Advert = advert;
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }
            
            // check if channel already exist
            var channelExists = _repository.Linq<ChatChannel>()
                .FirstOrDefault(x => x.Advert.Id == model.AdvertId && Enumerable.Any<ChatMember>(x.MemberCollection, y => y.User.Id == User.Identity.GetId()));
            var currentUser = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            if (channelExists != null)
            {
                var addMessage = new Chat
                {
                    Channel = channelExists,
                    Message = model.Message,
                    User = currentUser
                };
            
                _repository.Add(addMessage);
                
                PushMessage(advert?.User, model.Message, channelExists.Id);
            
                return Json(new ModelResponse
                {
                    Action = Actions.Redirect,
                    Url = Url.Action("Channel", "Chats", new { id = channelExists.Id })
                });
            }
            
            var newChannel = new ChatChannel
            {
                Advert = advert,
                Title = $"Advert: {advert?.Title}"
            };

            _repository.Add(newChannel);
            
            
            var chatMember = new ChatMember
            {
                User = currentUser,
                Channel = newChannel
            };
            
            _repository.Add(chatMember);
            
            var seller = new ChatMember
            {
                User = advert?.User,
                IsSeller = true,
                Channel = newChannel
            };
            
            _repository.Add(seller);
            
            PushMessage(advert?.User, model.Message, newChannel.Id);
            
            // add message 
            var chatMessage = new Chat
            {
                Channel = newChannel,
                Message = model.Message,
                User = currentUser,
                isread = false,
            };
            
            _repository.Add(chatMessage);
            
            return Json(new ModelResponse
            {
                Action = Actions.Redirect,
                Url = Url.Action("Channel", "Chats", new { id = newChannel.Id })
            });
        }

        private void PushMessage(User user, string message, int channel = 0)
        {
            foreach (var token in user.Tokens)
            {
                var pushMessage = new FirebasePushMessage
                {
                    CollapseKey = "New Message",
                    Priority = "high",
                    To = token.Token,
                    Notification = new Subby.Web.Models.FIrebaseViewModels.Notification
                    {
                        Title = "You have a message",
                        Body = message
                    },
                    Data = new Data
                    {
                        Type = "OPEN_CHAT",
                        Channel = channel,
                        ClickAction = "FLUTTER_NOTIFICATION_CLICK"
                    }
                };

                var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "key=AAAAmmNT_wQ:APA91bHEe3rCs0EF2KD6XhP2jGHj01q4U3bcFu9aOggnxcX-mKSNNaRKQ-W8flECC3bNgzEdA69ke6J6KY1oY1ybFF8V0DPg7pabYa99N3PD7FNbQoSo4rstIld0AvseqEPVwFlVowl9AGrTPQQoX-ywTHWlS9Qk2A");
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(JsonConvert.SerializeObject(pushMessage));
                var response = client.Execute(request);

            }
        }

        private IActionResult Filled(Chat model)
        {
            //var advert = _repository.Linq<Chat>().FirstOrDefault(x => x.isread == model.Id);

            if (model != null)
            {
                model.isread = true;
                _repository.Update(model);
            }

            return Ok();
        }
    }
}
