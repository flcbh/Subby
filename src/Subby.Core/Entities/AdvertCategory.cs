using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class AdvertCategory : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Slug { get; set; }
        public virtual bool Active { get; set; } = true;
    }
}