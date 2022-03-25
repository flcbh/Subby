using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Session : BaseEntity
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime Expiry { get; set; }
    }
}