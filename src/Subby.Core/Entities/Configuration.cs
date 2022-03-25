using System;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Configuration : BaseEntity
    {
        public virtual string Description { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
    }
}