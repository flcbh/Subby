using Subby.Data;

namespace SubbyNetwork.Models
{
    public class SubscriptionViewModel
    {
        public bool IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<Benefit> Benefits { get; set; }
    }
}