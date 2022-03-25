using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class User : BaseEntity
    {
        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual string UserName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string NormalizedUserName { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        
        public virtual bool MarketingOptIn { get; set; }
        
        public virtual string Bio { get; set; }
        
        public virtual bool IsTrader { get; set; }
        
        public virtual string TradeName { get; set; }
        
        public virtual string TradeAddressLine1 { get; set; }
        
        public virtual string TradeAddressLine2 { get; set; }
        
        public virtual string TradeCity { get; set; }
        
        public virtual string TradePostcode { get; set; }
        
        public virtual decimal TradeDistance { get; set; }
        
        public virtual ICollection<UserToken> Tokens { get; set; }
        public virtual ICollection<UserTrade> Trades { get; set; }
        
        public virtual ICollection<JobInterest> Interests { get; set; }
        
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        
        public virtual ICollection<JobApplication> Applications { get; set; }
        
        public virtual ICollection<UserReview> UserReviews { get; set; }
        
        public virtual double? Latitude { get; set; }
        
        public virtual double? Longitude { get; set; }
        
        public virtual string Avatar { get; set; }
        
        public virtual string Role { get; set; }

        public virtual bool ShowExternalJobs { get; set; } = true;
        public virtual bool IsPremium => Subscriptions.Any(x => x.ExpiryDate >= DateTime.Now);
        
        public virtual string PushToken { get; set; }
        
        public virtual DateTime? LastActive { get; set; }
    }
}