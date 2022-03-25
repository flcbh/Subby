using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class UserReview : BaseEntity
    {
        public virtual User User { get; set; }
        public virtual Job Job { get; set; }
        public virtual string Review { get; set; }
        public virtual DateTime DateCompleted { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual int Tidiness { get; set; }
        public virtual int Reliability { get; set; }
        public virtual int Courtesy { get; set; }
        public virtual User Reviewer { get; set; }
    }
}