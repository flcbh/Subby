using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Chat : BaseEntity
    {
        public virtual User User { get; set; }
        public virtual ChatChannel Channel { get; set; }
        public virtual string Message { get; set; }
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual bool isread { get; set; } = false;
    }
}