using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class UserToken : BaseEntity
    {
        public virtual User User { get; set; }
        public virtual string Token { get; set; }
    }
}