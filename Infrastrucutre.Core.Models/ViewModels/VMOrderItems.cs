using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class VMOrderItems
    {
        public int OrderId { get; set; }
        public double ProductSalesPrice { get; set; }
        public double ProductSalesTax { get; set; }
        public double PostageCredits { get; set; }
        public double ShippingCreditsTax { get; set; }
        public double GiftWrapCredits { get; set; }
        public double GiftWrapCreditsTax { get; set; }
        public double PromotionalRebates { get; set; }
        public double PromotionalRebatesTax { get; set; }
        public double MarketplaceWithHeldTax { get; set; }
        public double SellingFees { get; set; }
        public double FbaFees { get; set; }
        public double OtherTransactionFees { get; set; }
        public double Other { get; set; }

        public double ItemCost { get; set; }
        public double ActualDelivery { get; set; }
    }
}
