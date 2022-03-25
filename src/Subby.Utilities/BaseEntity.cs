using System.Collections.Generic;

namespace Subby.Utilities
{
    // This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }
    }
}