using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class JobActivity  : BaseEntity
    {
        public virtual int Count { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual Job Job { get; set; }
    }
}