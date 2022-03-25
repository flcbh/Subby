using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Sponsor : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Logo { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Website { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime LiveDate { get; set; }
    }
}