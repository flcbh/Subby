using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class JobTrade : BaseEntity
    {
        public virtual Job Job { get; set; }
        public virtual Trade Trade { get; set; }
    }
}