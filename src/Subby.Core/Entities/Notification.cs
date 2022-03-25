using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Notification : BaseEntity
    {
        public virtual User User { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
        public virtual bool Read { get; set; }
        public virtual DateTime ReadAt { get; set; }
        public virtual DateTime CreatedAt { get; set; }
    }
}