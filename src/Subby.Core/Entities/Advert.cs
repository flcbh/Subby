using System;
using System.Collections.Generic;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Advert : BaseEntity
    {
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual string ExternalLink { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Location { get; set; }
        public virtual string City { get; set; }
        public virtual string Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string Unit { get; set; }
        public virtual string Condition { get; set; }
        public virtual bool IsSold { get; set; } = false;
        public virtual bool IsFree { get; set; } = false;
        public virtual string Media { get; set; }
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual AdvertCategory Category { get; set; }
        public virtual User User { get; set; }
        public virtual string Slug { get; set; }
        public virtual ICollection<ChatChannel> ChatCollection { get; set; }
        public virtual ICollection<Media> MediaCollection { get; set; }

    }
}