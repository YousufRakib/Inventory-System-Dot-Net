using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class ItemStockViewModel
    {
        public string ItemMasterID { get; set; }
        public string WhStock { get; set; }
        public string ARSUKFBAStock { get; set; }
        public string NEEZFBAStock { get; set; }
        public string FBAStock { get; set; }
        public string TotalStock { get; set; }

        public string CurrentYearSold { get; set; }
        public string LastYearSold { get; set; }
        public string TargetSellingAvg { get; set; }
        public string ActualTargetQty { get; set; }
        public string IncrementSold { get; set; }
        public decimal ExtraStock { get; set; }
        public string CurrentStockSurvivalDays { get; set; }
        public decimal ActualForecastingDays { get; set; }
        public decimal OrderReceivingDaysBeforeSelling { get; set; }
        public decimal TotalQtyNeedToOrder { get; set; }
        public string OrderStatus { get; set; }
        public decimal LessPeriod { get; set; }
        public string StockDaysStatus { get; set; }
        public string ExtendPeriod { get; set; }
        public decimal TotalForecastingDays { get; set; }
        public decimal TotalQtyNeed { get; set; }


        public string SellerID { get; set; }
        public string SKU { get; set; }
        public string FNSKU { get; set; }
        public decimal QuantitySold { get; set; }
        public decimal TotalSold { get; set; }
        public string AvgSoldPercentage { get; set; }
        public decimal AvgSold { get; set; }
        public string TotalQtyNeeded { get; set; }
        public decimal OrderByFNSKU { get; set; }
        public decimal FinalOrder { get; set; }
        public string QtyPerCarton { get; set; }
        public decimal TotalCarton { get; set; }
        public string BoxDim { get; set; }
        public string CBM3 { get; set; }
        public decimal Total { get; set; }
    }
}
