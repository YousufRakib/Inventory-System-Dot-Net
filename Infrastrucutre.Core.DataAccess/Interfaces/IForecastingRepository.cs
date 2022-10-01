using Infrastrucutre.Core.Models.Models;
using Infrastrucutre.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.DataAccess.Interfaces
{
    public interface IForecastingRepository
    {
        List<ForecastingViewModel> GetForecastingViewData(string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string forecastingPeriod, string increment, string depositDays, string manufacturePeriodAndShipingDays, string filterText);
        ItemStockViewModel GetItemsStock(int fbaRootId, int itemMasterId);
        ItemStockViewModel GetsItemForecastingInfo(int fbaRootId, int itemMasterId, string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string incrementTarget, string averageDurationDays);
        List<ItemStockViewModel> GetsItemForecastingListData(int? itemMasterId, decimal? currentYearSold, string currentStartDate, string currentEndDate, int? warehouseStockId, decimal? totalQtyNeedToOrder);
        string GetOrderId();
        string GetSupplierOrderId();
        List<ShipmentViewModel> GetShipmentHistory(string id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0);
        string SaveItemForecasting(ForecastingItemViewModel forecastingItemViewModel);
        string SaveSupplierOrder(SupplierOrderViewModel supplierOrderViewModel);
        string SaveCreateShipment(CreateShipmentViewModel createShipmentViewModel);
        List<ForecastingItemSummary> GetForecastingItemsSummaryList();
        List<ShipmentViewModel> ShipmentHistoryDownload(string id);
        List<SupplierOrderSummary> GetSupplierOrderList();
        List<CreateShipmentSummary> GetCreateShipmentList();
        bool RemoveForecastingItem(int id);
        bool RemoveSupplierOrder(int id);
        bool RemoveCreateShipment(int id);
        ForecastingItemSummary GetItemForecastingByID(int Id);
        SupplierOrderSummary GetSupplierOrderByID(int Id);
        CreateShipmentSummary GetCreateShipmentByID(int Id);
        List<ForecastingItem> GetsItemForecastingListDataById(int? id);
        string UpdateItemForecasting(ForecastingItemViewModel forecastingItemViewModel);
        string UpdateCreateShipment(CreateShipmentViewModel createShipmentViewModel);
        List<ForecastingItemSummary> GetsItemForecastingSummaryListForSupplierOrder(out int totalCount, string warehouseRoot = "", string supplierName = "", int jtStartIndex = 0, int jtPageSize = 0);
        List<SupplierOrderSummary> GetSupplierOrderListWithWarehouse(out int totalCount, string warehouseRoot = "", int jtStartIndex = 0, int jtPageSize = 0);
        List<ForecastingItemSummary> GetSupplierNameFromForecasting();
        List<SupplierOrders> GetSupplierOrderListById(int? id);
        List<CreateShipment> GetCreateShipmentListById(int? id);
        string GetShipmentOrderId();
    }
}
