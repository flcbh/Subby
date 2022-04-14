using System.Collections.Generic;

namespace SubbyNetwork.Models.PaymentViewModels
{
    public class PaymentPlanViewModel
    {
        public string PublicKey { get; set; }
        public List<PaymentPlan> Plans { get; set; } = new List<PaymentPlan>();
    }

    public class PaymentPlan
    {
        public string Key { get; set; }
        public decimal Amount { get; set; }
    }
}