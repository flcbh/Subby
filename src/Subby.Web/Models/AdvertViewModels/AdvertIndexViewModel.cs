using System.Collections.Generic;
using Subby.Core.Entities;

namespace Subby.Web.Models.AdvertViewModels
{
    public class AdvertIndexViewModel<T>
    {
        public ICollection<AdvertCategory> Categories { get; set; }
        public int CategoryId { get; set; }
        public LastContent.Utilities.Pagination.PagedResult<T> Items { get; set; }
        public bool IsFree { get; set; }
        public bool IsSold { get; set; }
        public int messages { get; set; }
        public int miles { get; set; }
    }
}