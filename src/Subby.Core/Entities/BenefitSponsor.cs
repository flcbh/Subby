using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class BenefitSponsor : BaseEntity
    {
        public virtual Benefit Benefit { get; set; }
        public virtual Sponsor Sponsor { get; set; }
    }
}