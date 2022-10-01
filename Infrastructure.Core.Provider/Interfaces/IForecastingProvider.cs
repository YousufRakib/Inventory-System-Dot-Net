using Infrastrucutre.Core.Models.Models;
using Infrastrucutre.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Provider.Interfaces
{
    public interface IForecastingProvider
    {
        List<ForecastingViewModel> GetForecastingViewData(string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string forecastingPeriod, string increment, string depositDays, string manufacturePeriodAndShipingDays, string filterText);
        ItemStockViewModel GetItemsStock(int fbaRootId, int itemMasterId);
        ItemStockViewModel GetsItemForecastingInfo(int fbaRootId, int itemMasterId, string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string incrementTarget, string averageDurationDays);
        List<ItemStockViewModel> GetsItemForecastingListData(int? itemMasterId, decimal? currentYearSold, string currentStartDate, string currentEndDate, int? warehouseStockId, decimal? totalQtyNeedToOrder);
        string GetOrderId();
        string SaveItemForecasting(ForecastingItemViewModel forecastingItemViewModel);
        List<ForecastingItemSummary> GetForecastingItemsSummaryList();
        bool RemoveForecastingItem(int id);
        ForecastingItemSummary GetItemForecastingByID(int Id);
        List<ForecastingItem> GetsItemForecastingListDataById(int? id);
        string UpdateItemForecasting(ForecastingItemViewModel forecastingItemViewModel);
        string UpdateCreateShipment(CreateShipmentViewModel createShipmentViewModel);
        List<ForecastingItemSummary> GetsItemForecastingSummaryListForSupplierOrder(out int totalCount, string warehouseRoot = "", string supplierName = "", int jtStartIndex = 0, int jtPageSize = 0);
        List<ForecastingItemSummary> GetSupplierNameFromForecasting();
        string GetSupplierOrderId();
        string SaveSupplierOrder(SupplierOrderViewModel supplierOrderViewModel);
        List<SupplierOrderSummary> GetSupplierOrderList();
        List<CreateShipmentSummary> GetCreateShipmentList();
        List<ShipmentViewModel> ShipmentHistoryDownload(string id);
        List<ShipmentViewModel> GetShipmentHistory(string id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0);
        List<SupplierOrderSummary> GetSupplierOrderListWithWarehouse(out int totalCount, string warehouseRoot = "", int jtStartIndex = 0, int jtPageSize = 0);
        bool RemoveSupplierOrder(int id);
        bool RemoveCreateShipment(int id);
        SupplierOrderSummary GetSupplierOrderByID(int Id);
        CreateShipmentSummary GetCreateShipmentByID(int Id);
        List<SupplierOrders> GetSupplierOrderListById(int? id);
        List<CreateShipment> GetCreateShipmentListById(int? id);
            string GetShipmentOrderId();
        string SaveCreateShipment(CreateShipmentViewModel createShipmentViewModel);
    }
}
