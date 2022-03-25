using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Job : BaseEntity
    {
        public virtual string Title { get; set; }
        
        public virtual string Description { get; set; }

        public virtual string Location { get; set; }
        
        public virtual string DeadLine { get; set; }
        
        public virtual DateTime CreatedAt { get; set; }
        
        public virtual DateTime UpdatedAt { get; set; }

        public virtual double Latitude { get; set; }
        
        public virtual double Longitude { get; set; }
        
        public virtual string ExternalLink { get; set; }
        
        public virtual string Postcode { get; set; }
        
        public virtual string Slug { get; set; }

        public virtual string Budget { get; set; }
        public virtual bool IsFeatured { get; set; }
        public virtual string ContractType { get; set; }
        public virtual string Media { get; set; }

        public virtual int ViewCount
        {
            get { return Activities.Sum(x => x.Count); }
        }

        public virtual int Distance { get; set; }

        public virtual bool IsApplied { get; set; }

        public virtual bool IsExternal { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<JobActivity> Activities { get; set; }
        public virtual ICollection<JobApplication> Applications { get; set; }
        public virtual ICollection<JobInterest> JobInterests { get; set; }
        public virtual Trade Trade { get; set; }
        public virtual bool IsFilled { get; set; } = false;
        
        public virtual string Source { get; set; }
        public virtual string Reference { get; set; }
    }
}