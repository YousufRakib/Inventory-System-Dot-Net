using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.Models
{
    public class ForecastingItem
    {
        public long  Id { get; set; }
        public long  OrderId { get; set; }
        public int  ItemMasterID { get; set; }
        public string SellerID { get; set; }
        public string SKU { get; set; }
        public string FNSKU { get; set; }
        public int  QuantitySold { get; set; }
        public int  TotalSold { get; set; }
        public decimal  AvgSoldPercentage { get; set; }
        public decimal  AvgSold { get; set; }
        public decimal  TotalQtyNeeded { get; set; }
        public decimal  OrderByFNSKU { get; set; }
        public int  FinalOrder { get; set; }
        public int  QtyPerCarton { get; set; }
        public decimal  TotalCarton { get; set; }
        public string BoxDim { get; set; }
        public decimal  CBM3 { get; set; }
        public decimal  Total { get; set; }
        public string CreatedBy { get; set; }
        public DateTime  CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime  UpdatedDate { get; set; }
    }
}
