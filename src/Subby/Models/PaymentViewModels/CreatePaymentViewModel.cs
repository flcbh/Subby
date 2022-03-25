using System.Collections;
using System.Collections.Generic;

namespace Subby.Web.New.Models.PaymentViewModels
{
    public class CreatePaymentViewModel
    {
        public string Currency { get; set; }
        public string BusinessName { get; set; }
        public string ProductName { get; set; }
        public string CustomerEmail { get; set; }
        public ICollection<PaymentItemViewModel> Items { get; set; }
    }

    public class PaymentItemViewModel
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }
    }
}