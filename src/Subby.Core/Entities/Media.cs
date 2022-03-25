using System;
using System.Collections;
using System.Collections.Generic;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Media : BaseEntity
    {
        public virtual string Alt { get; set; }
        public virtual int Position { get; set; }
        public virtual int Height { get; set; }
        public virtual string MediaType { get; set; }
        public virtual string Src { get; set; }
        public virtual int Width { get; set; }
        public virtual string MimeType { get; set; }
        public virtual int Duration { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual ICollection<MediaMeta> MediaMetaCollection { get; set; } = new List<MediaMeta>();
        public virtual Advert Advert { get; set; }
    }
}