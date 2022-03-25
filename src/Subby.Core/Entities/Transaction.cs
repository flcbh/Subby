using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Transaction : BaseEntity
    {
        public enum Statuses
        {
            Succeeded, Failed, Pending
        }

        public virtual decimal Amount { get; set; }

        public virtual string Reference { get; set; }

        public virtual string ExternalReference { get; set; }

        public virtual string Secret { get; set; }

        public virtual string Token { get; set; }

        public virtual string Uuid { get; set; }

        public virtual Statuses Status { get; set; }

        public virtual string Response { get; set; }

        public virtual string PaymentProvider { get; set; }

        public virtual DateTime CreatedAt { get; set; }
        
        public virtual string Currency { get; set; }
    }
}