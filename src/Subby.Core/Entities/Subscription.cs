using System;
using System.Collections.Generic;
using Subby.Utilities;

namespace Subby.Core.Entities
{ 
    public class Subscription : BaseEntity
    {
        public virtual decimal Amount { get; set; }
        public virtual User User { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual Transaction Transaction { get; set; }
        public virtual string Plan { get; set; }
    }
}