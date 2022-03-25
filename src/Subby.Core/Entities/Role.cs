using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Role : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }
    }
}