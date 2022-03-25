using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class ChatMember : BaseEntity
    {
        public virtual ChatChannel Channel { get; set; }
        public virtual User User { get; set; }
        public virtual bool IsSeller { get; set; }
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual bool IsLeft { get; set; } = false;
    }
}