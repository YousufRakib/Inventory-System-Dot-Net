using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class ForecastingViewModel
    {
        public string ItemName { get; set; }
        public string MarketPlace { get; set; }
        public string CurrentYearSold { get; set; }
        public string LastYearSold { get; set; }
        public string TargetSellingAvg { get; set; }
        public string ForecastingPeriod { get; set; }
        public string AvgSold { get; set; }
        public string Increment { get; set; }
        public string ActualTargetSellingQty { get; set; }
        public string WarehouseStock { get; set; }
        public string FBAStock { get; set; }
        public string TotalStock { get; set; }
        public string SoldPerDay { get; set; }
        public string DepositDays { get; set; }
        public string ManufacturePeriodAndShippingDays { get; set; }
        public string CurrentStockSurvivalDays { get; set; }
        public string WithoutStockDays { get; set; }
        public string OrderReceivingDaysBeforeSelling { get; set; }
        public string ActualForecastingDays { get; set; }
        public string StockRequired { get; set; }
        public string TotalQtyNeedToOrder { get; set; }
        public string OrderStatus { get; set; }
        public string StockTargetPeriodDays { get; set; }
        public string StatusOfPeriod { get; set; }
    }
}
