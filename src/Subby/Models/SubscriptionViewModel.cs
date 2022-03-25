using System;
using System.Collections.Generic;
using Subby.Core.Entities;

namespace Subby.Web.New.Models
{
    public class SubscriptionViewModel
    {
        public bool IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<Benefit> Benefits { get; set; }
    }
}