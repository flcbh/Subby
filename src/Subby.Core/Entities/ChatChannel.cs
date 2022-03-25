using System;
using System.Collections.Generic;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class ChatChannel : BaseEntity
    {
        public virtual string Title { get; set; }
        public virtual Advert Advert { get; set; }
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual bool Active { get; set; } = true;
        public virtual ICollection<Chat> ChatCollection { get; set; }
        public virtual ICollection<ChatMember> MemberCollection { get; set; }
    }
}