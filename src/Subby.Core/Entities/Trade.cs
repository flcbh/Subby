using System.ComponentModel.DataAnnotations;
using Subby.Utilities;

namespace Subby.Core.Entities
{
    public class Trade : BaseEntity
    {
        [Required]
        public virtual string Name { get; set; }
        public virtual string Slug { get; set; }
        public virtual bool Active { get; set; } = true;
    }
}