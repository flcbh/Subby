using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class JobInterest : BaseEntity
    {
        public virtual Job Job { get; set; }
        public virtual User User { get; set; }
        public virtual bool IsInterested { get; set; }
        public virtual DateTime CreatedAt { get; set; }
    }
}