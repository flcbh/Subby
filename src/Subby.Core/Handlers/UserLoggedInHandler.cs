using System;
using System.Linq;
using Ardalis.GuardClauses;
using Subby.Core.Entities;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Utilities.Interfaces;

namespace Subby.Core.Handlers
{
    public class UserLoggedInHandler : IHandle<UserLoggedInEvent>
    {
        private readonly IRepository _repository;
        public UserLoggedInHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(UserLoggedInEvent domainEvent)
        {
            Guard.Against.Null(domainEvent, nameof(domainEvent));
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == domainEvent.Email);

            if (user != null)
            {
                user.LastActive = DateTime.Now;

                if (!string.IsNullOrEmpty(domainEvent.PushToken))
                {
                    user.PushToken = domainEvent.PushToken;
                    _repository.Add(new UserToken
                    {
                        User = user,
                        Token = domainEvent.PushToken
                    });
                }

                _repository.Update(user);   
            }
            
            
        }
    }
}