using System.Collections.Generic;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Benefit : BaseEntity
    {
        public virtual string Title { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Image { get; set; }
        public virtual string Position { get; set; }
        public virtual bool Active { get; set; }
        public virtual ICollection<BenefitSponsor> Sponsors { get; set; }
    }
}