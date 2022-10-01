using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class VMOrder
    {
        public string OrderReferenceNo { get; set; }
        public string MarketPlace { get; set; }
        public string Fulfilment { get; set; }
        public string TaxCollectionModel { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public int SellerIndex { get; set; }
        public string SellerID { get; set; }
    }
}
