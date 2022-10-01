using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class VM_OrderSheet_Upload
    { 
        public DateTime DateTime { get; set; }
        public string SettlementId { get; set; }
        public string Type { get; set; }
        public string OrderReferenceNo { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string MarketPlace { get; set; }
        public string Fulfilment { get; set; }
        public string OrderCity { get; set; }
        public string OrderState { get; set; }
        public string OrderPostal { get; set; }
        public string TaxCollectionModel { get; set; }
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
        public double total { get; set; }



    }
}
