using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Audit  : BaseEntity
    {
        public virtual string Description { get; set; }

        public virtual string IpAddress { get; set; }

        public virtual DateTime RequestTime { get; set; }

        public virtual long ResponseMillis { get; set; }

        public virtual int StatusCode { get; set; }

        public virtual string Method { get; set; }

        public virtual string Path { get; set; }

        public virtual string QueryString { get; set; }

        public virtual string RequestBody { get; set; }

        public virtual string ResponseBody { get; set; }

        public virtual string UserAgent { get; set; }

        public virtual string City { get; set; }

        public virtual decimal Latitude { get; set; }

        public virtual decimal Longitude { get; set; }

        public virtual string Browser { get; set; }

        public virtual int BrowserVersion { get; set; }
        
        public virtual bool Sandbox { get; set; }
        
        public virtual int TenantId { get; set; }
        
        public virtual string TenantName { get; set; }

        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}