using Subby.Data;
using System.Linq.Dynamic.Core;

namespace SubbyNetwork.Models.AdvertViewModels
{
    public class AdvertIndexViewModel<T>
    {
        public ICollection<AdvertCategory> Categories { get; set; }
        public int CategoryId { get; set; }
        public PagedResult<T> Items { get; set; }
        public bool IsFree { get; set; }
        public bool IsSold { get; set; }
        public int messages { get; set; }
        public int miles { get; set; }
    }
}