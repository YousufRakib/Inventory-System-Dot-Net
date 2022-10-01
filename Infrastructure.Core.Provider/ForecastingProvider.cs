using Infrastructure.Core.Provider.Interfaces;
using Infrastrucutre.Core.DataAccess.Interfaces;
using Infrastrucutre.Core.Models.Models;
using Infrastrucutre.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Provider
{
    public class ForecastingProvider : IForecastingProvider
    {
        IForecastingRepository _forecastingRepository;

        public ForecastingProvider(IForecastingRepository forecastingRepository)
        {
            this._forecastingRepository = forecastingRepository;
        }

        public List<ForecastingViewModel> GetForecastingViewData(string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string forecastingPeriod, string increment, string depositDays, string manufacturePeriodAndShipingDays, string filterText)
        {
            return _forecastingRepository.GetForecastingViewData(currentStartDate, currentEndDate, lastYearStartDate, lastYearEndDate, forecastingPeriod, increment, depositDays, manufacturePeriodAndShipingDays, filterText);
        }

        public ItemStockViewModel GetItemsStock(int fbaRootId, int itemMasterId)
        {
            return _forecastingRepository.GetItemsStock(fbaRootId, itemMasterId);
        }

        public ItemStockViewModel GetsItemForecastingInfo(int fbaRootId, int itemMasterId, string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string incrementTarget, string averageDurationDays)
        {
            return _forecastingRepository.GetsItemForecastingInfo(fbaRootId, itemMasterId, currentStartDate, currentEndDate, lastYearStartDate, lastYearEndDate, incrementTarget, averageDurationDays);
        }

        public List<ItemStockViewModel> GetsItemForecastingListData(int? itemMasterId, decimal? currentYearSold, string currentStartDate, string currentEndDate, int? warehouseStockId, decimal? totalQtyNeedToOrder)
        {
            return _forecastingRepository.GetsItemForecastingListData(itemMasterId, currentYearSold,currentStartDate,currentEndDate,warehouseStockId,totalQtyNeedToOrder);
        }

        public List<ForecastingItem> GetsItemForecastingListDataById(int? id)
        {
            return _forecastingRepository.GetsItemForecastingListDataById(id);
        }

        public string GetOrderId()
        {
            return _forecastingRepository.GetOrderId();
        }
        public string SaveItemForecasting(ForecastingItemViewModel forecastingItemViewModel)
        {
            return _forecastingRepository.SaveItemForecasting(forecastingItemViewModel);
        }

        public string UpdateItemForecasting(ForecastingItemViewModel forecastingItemViewModel)
        {
            return _forecastingRepository.UpdateItemForecasting(forecastingItemViewModel);
        }

        public string UpdateCreateShipment(CreateShipmentViewModel createShipmentViewModel)
        {
            return _forecastingRepository.UpdateCreateShipment(createShipmentViewModel);
        }

        public List<ForecastingItemSummary> GetForecastingItemsSummaryList()
        {
            return _forecastingRepository.GetForecastingItemsSummaryList();
        }

        public bool RemoveForecastingItem(int id)
        {
            return _forecastingRepository.RemoveForecastingItem(id);
        }

        public ForecastingItemSummary GetItemForecastingByID(int Id)
        {
            return _forecastingRepository.GetItemForecastingByID(Id);
        }

        public List<ForecastingItemSummary> GetsItemForecastingSummaryListForSupplierOrder(out int totalCount, string warehouseRoot = "", string supplierName = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _forecastingRepository.GetsItemForecastingSummaryListForSupplierOrder(out totalCount, warehouseRoot,supplierName, jtStartIndex, jtPageSize);
        }

        public List<ForecastingItemSummary> GetSupplierNameFromForecasting()
        {
            return _forecastingRepository.GetSupplierNameFromForecasting();
        }

        public string GetSupplierOrderId()
        {
            return _forecastingRepository.GetSupplierOrderId();
        }

        public string SaveSupplierOrder(SupplierOrderViewModel supplierOrderViewModel)
        {
            return _forecastingRepository.SaveSupplierOrder(supplierOrderViewModel);
        }

        public List<SupplierOrderSummary> GetSupplierOrderList()
        {
            return _forecastingRepository.GetSupplierOrderList();
        }

        public List<CreateShipmentSummary> GetCreateShipmentList()
        {
            return _forecastingRepository.GetCreateShipmentList();
        }

        public List<ShipmentViewModel> GetShipmentHistory(string id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _forecastingRepository.GetShipmentHistory(id,out rowCount, jtStartIndex, jtPageSize);
        }

        public List<ShipmentViewModel> ShipmentHistoryDownload(string id)
        {
            return _forecastingRepository.ShipmentHistoryDownload(id);
        }

        public List<SupplierOrderSummary> GetSupplierOrderListWithWarehouse(out int totalCount, string warehouseRoot = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _forecastingRepository.GetSupplierOrderListWithWarehouse(out totalCount, warehouseRoot, jtStartIndex, jtPageSize);
        }

        public bool RemoveSupplierOrder(int id)
        {
            return _forecastingRepository.RemoveSupplierOrder(id);
        }

        public bool RemoveCreateShipment(int id)
        {
            return _forecastingRepository.RemoveCreateShipment(id);
        }

        public SupplierOrderSummary GetSupplierOrderByID(int Id)
        {
            return _forecastingRepository.GetSupplierOrderByID(Id);
        }

        public CreateShipmentSummary GetCreateShipmentByID(int Id)
        {
            return _forecastingRepository.GetCreateShipmentByID(Id);
        }

        public List<SupplierOrders> GetSupplierOrderListById(int? id)
        {
            return _forecastingRepository.GetSupplierOrderListById(id);
        }

        public List<CreateShipment> GetCreateShipmentListById(int? id)
        {
            return _forecastingRepository.GetCreateShipmentListById(id);
        }

        public string GetShipmentOrderId()
        {
            return _forecastingRepository.GetShipmentOrderId();
        }

        public string SaveCreateShipment(CreateShipmentViewModel createShipmentViewModel)
        {
            return _forecastingRepository.SaveCreateShipment(createShipmentViewModel);
        }
    }
}
