using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class UserTrade : BaseEntity
    {
        public virtual User User { get; set; }
        public virtual Trade Trade { get; set; }
    }
}