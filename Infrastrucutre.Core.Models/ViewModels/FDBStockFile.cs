using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class FDBStockFile
    {
        public long Id { get; set; }
        public string Sku { get; set; }
        public string Fnsku { get; set; }
        public string Asin { get; set; }
        public string ProductName { get; set; }
        public string Condition { get; set; }
        public decimal? YourPrice { get; set; }
        public string MfnListingExists { get; set; }
        public int? MfnFulfillableQuantity { get; set; }
        public string AfnListingExists { get; set; }
        public int? AfnWarehouseQuantity { get; set; }
        public int? AfnFulfillableQuantity { get; set; }
        public int? AfnUnsellableQuantity { get; set; }
        public int? AfnReservedQuantity { get; set; }
        public int? AfnTotalQuantity { get; set; }
        public decimal? PerUnitVolume { get; set; }
        public int? AfnInboundWorkingQuantity { get; set; }
        public int? AfnInboundShippedQuantity { get; set; }
        public int? AfnInboundReceivingQuantity { get; set; }
        public int? AfnResearchingQuantity { get; set; }
        public int? AfnReservedFutureSupply { get; set; }
        public int? AfnFutureSupplyBuyable { get; set; }
        public string Location { get; set; }
        public string Seller { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
