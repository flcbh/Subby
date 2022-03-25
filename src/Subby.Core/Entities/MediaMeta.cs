using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class MediaMeta : BaseEntity
    {
        public virtual string Key { get; set; }

        public virtual string Value { get; set; }
        public virtual Media Media { get; set; }
    }
}