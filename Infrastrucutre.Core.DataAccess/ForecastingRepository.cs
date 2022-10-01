using Dapper;
using DapperExtensions;
using Infrastructure.ConfigurationProvider;
using Infrastructure.Core.DataAccess;
using Infrastrucutre.Core.DataAccess.Interfaces;
using Infrastrucutre.Core.Models;
using Infrastrucutre.Core.Models.Models;
using Infrastrucutre.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.DataAccess
{
    public class ForecastingRepository : IForecastingRepository
    {
        public List<ForecastingViewModel> GetForecastingViewData(string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string forecastingPeriod, string increment, string depositDays, string manufacturePeriodAndShipingDays, string filterText)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                if (filterText != "")
                {
                    var parameters = new DynamicParameters();
                    string query = string.Format("Select TOP 1 (ItemName + '-' + ItemCode) as label,ItemMasterID as id from ItemMasters Where IsActive=1  AND itemName LIKE '{0}%'", filterText);

                    IEnumerable<AutoCompleteItem> items = connection.Query<AutoCompleteItem>(query, parameters, commandType: CommandType.Text);
                    var itemMasterId = items.Select(x => x.id).FirstOrDefault();

                    var result = connection.Query<ForecastingViewModel>("ForecastingViewReportWithItem", new { FromDateCurrentYear = currentStartDate, ToDateCurrentYear = currentEndDate, FromDateLastYear = lastYearStartDate, ToDateLastYear = lastYearEndDate, ForecastingPeriod = forecastingPeriod, Increment = increment, DepositDays = depositDays, ManufacturePeriodAndShippingDays = manufacturePeriodAndShipingDays, ItemMasterId = itemMasterId }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    return result.ToList();
                }
                else
                {

                    var result = connection.Query<ForecastingViewModel>("ForecastingViewReportWithoutItem", new { FromDateCurrentYear = currentStartDate, ToDateCurrentYear = currentEndDate, FromDateLastYear = lastYearStartDate, ToDateLastYear = lastYearEndDate, ForecastingPeriod = forecastingPeriod, Increment = increment, DepositDays = depositDays, ManufacturePeriodAndShippingDays = manufacturePeriodAndShipingDays }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    return result.ToList();
                }
            }
        }

        public ItemStockViewModel GetItemsStock(int fbaRootId, int itemMasterId)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                //string wareHouse = "";

                //if (fbaRootId == 1) { wareHouse = "UK"; }
                //else if (fbaRootId == 2) { wareHouse = "EU"; }
                //else if (fbaRootId == 3) { wareHouse = "US"; }
                //else if (fbaRootId == 4) { wareHouse = "AU"; }
                //else if (fbaRootId == 5) { wareHouse = "CA"; }

                //string queryStock = string.Format(@"SELECT * from StockViewForStockManagements where ItemMasterId= {0} and FBARootID={1};", itemMasterId, fbaRootId);
                //var readerStockView = connection.QueryMultiple(queryStock);
                //var resultStockView = readerStockView.Read<StockViewForStockManagement>();

                //string queryFBAStock = string.Format(@"select distinct SIL.ItemMasterID,SIL.SKU,FDBSF.Location,FDBSF.Seller, FDBSF.AfnTotalQuantity from SellerItemLink as SIL
                //                                    inner join FDBStockFile as FDBSF on SIL.SKU=FDBSF.Sku
                //                                    where SIL.ItemMasterID ={0} And FDBSF.Seller in ('ARSUK','NEEZ') And FDBSF.Location='{1}'
                //                                    AND FDBSF.Sku is not null", itemMasterId, wareHouse);

                //var readerFBAStock = connection.QueryMultiple(queryFBAStock);
                //var resultFBAStock = readerFBAStock.Read<FDBStockQueryResult>();

                //ItemStockViewModel itemStockResult = new ItemStockViewModel();

                //itemStockResult.WhStock = GetWHStock(resultStockView, fbaRootId, itemMasterId);
                //itemStockResult.ARSUKFBAStock = GetFBAStock(resultFBAStock, "ARSUK", wareHouse, itemMasterId);
                //itemStockResult.NEEZFBAStock = GetFBAStock(resultFBAStock, "NEEZ", wareHouse, itemMasterId);
                //itemStockResult.FBAStock = GetFBAStock(itemStockViewModel.ARSUKFBAStock, itemStockViewModel.NEEZFBAStock);
                //itemStockResult.TotalStock = GetTotalStock(itemStockViewModel.WhStock, itemStockViewModel.ARSUKFBAStock, itemStockViewModel.NEEZFBAStock);

                var result = connection.Query<ItemStockViewModel>("GetItemStock", new { FBARootId = fbaRootId, ItemMasterId = itemMasterId }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                ItemStockViewModel itemStockViewModel = result.FirstOrDefault();

                ItemStockViewModel itemStockResult = new ItemStockViewModel();

                itemStockResult.WhStock = itemStockViewModel.WhStock;
                itemStockResult.FBAStock = itemStockViewModel.FBAStock;
                itemStockResult.TotalStock = (Convert.ToInt32(itemStockViewModel.WhStock) + Convert.ToInt32(itemStockViewModel.FBAStock)).ToString();

                return itemStockResult;
            }
        }

        public ItemStockViewModel GetsItemForecastingInfo(int fbaRootId, int itemMasterId, string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string incrementTarget, string averageDurationDays)
        {
            string wareHouse = "";

            if (fbaRootId == 1) { wareHouse = "UK"; }
            else if (fbaRootId == 2) { wareHouse = "EU"; }
            else if (fbaRootId == 3) { wareHouse = "US"; }
            else if (fbaRootId == 4) { wareHouse = "AU"; }
            else if (fbaRootId == 5) { wareHouse = "CA"; }

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<ItemStockViewModel>("GetItemForecastingData", new { FromDateCurrentYear = currentStartDate, ToDateCurrentYear = currentEndDate, FromDateLastYear = lastYearStartDate, ToDateLastYear = lastYearEndDate, AvgDuration = averageDurationDays, Increment = incrementTarget, ItemMasterId = itemMasterId, WareHouse = wareHouse }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                ItemStockViewModel itemStockViewModel = result.FirstOrDefault();

                return itemStockViewModel;
            }
        }

        public List<ItemStockViewModel> GetsItemForecastingListData(int? itemMasterId, decimal? currentYearSold, string currentStartDate, string currentEndDate, int? warehouseStockId, decimal? totalQtyNeedToOrder)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<ItemStockViewModel>("GetItemForecastingListData", new { ItemMasterId = itemMasterId, CurrentYearSold = currentYearSold, FromDateCurrentYear = currentStartDate, ToDateCurrentYear = currentEndDate, MarketPlace = warehouseStockId, TotalQtyNeeded = totalQtyNeedToOrder }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                List<ItemStockViewModel> itemStockViewModel = result.ToList();

                return itemStockViewModel;
            }
        }

        public string GetOrderId()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("select top 1 * from ForecastingItemSummary order by OrderId desc");

                IEnumerable<ForecastingItemSummary> forecastingItem = connection.Query<ForecastingItemSummary>(query, commandType: CommandType.Text);

                var orderId = forecastingItem.Select(x => x.OrderId).FirstOrDefault();

                if (orderId == null)
                {
                    orderId = 1;
                }
                else
                {
                    orderId = Convert.ToInt64(orderId) + 1;
                }

                return orderId.ToString();
            }
        }

        public string GetShipmentOrderId()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("select top 1 * from CreateShipmentSummary order by ShipmentOrderId desc");

                IEnumerable<CreateShipmentSummary> createShipmentSummary = connection.Query<CreateShipmentSummary>(query, commandType: CommandType.Text);

                var shipmentOrderId = createShipmentSummary.Select(x => x.ShipmentOrderId).FirstOrDefault();

                if (shipmentOrderId == null)
                {
                    shipmentOrderId = 1;
                }
                else
                {
                    shipmentOrderId = Convert.ToInt64(shipmentOrderId) + 1;
                }

                return shipmentOrderId.ToString();
            }
        }

        public string GetSupplierOrderId()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("select top 1 * from SupplierOrderSummary order by SupplierOrderId desc");

                IEnumerable<SupplierOrderSummary> supplierOrderSummary = connection.Query<SupplierOrderSummary>(query, commandType: CommandType.Text);

                var supplierOrderId = supplierOrderSummary.Select(x => x.SupplierOrderId).FirstOrDefault();

                if (supplierOrderId == null)
                {
                    supplierOrderId = 1;
                }
                else
                {
                    supplierOrderId = Convert.ToInt64(supplierOrderId) + 1;
                }

                return supplierOrderId.ToString();
            }
        }

        public string SaveItemForecasting(ForecastingItemViewModel forecastingItemViewModel)
        {
            string result = "";
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(@"Insert INTO ForecastingItemSummary 
                                         (OrderId,ItemMasterId,SupplierName,ForecastingQty,TotalSoldQty,BatchNo,OrderDate,TotalCVM,SupplierCurrency,
                                         SupplierCost,TotalQty,TotalValue,TotalBox,PortOfLoading,LeadTime,Comments,SCMNameId,WarehouseRootId,SCMName,
                                         WarehouseRoot,CreatedBy,CreatedDate,IsActive,IsActiveSupplierOrder)
                                         VALUES(@OrderId,@ItemMasterId,@SupplierName,@ForecastingQty,@TotalSoldQty,@BatchNo,@OrderDate,@TotalCVM,
                                         @SupplierCurrency,@SupplierCost,@TotalQty,@TotalValue,@TotalBox,@PortOfLoading,@LeadTime,@Comments,@SCMNameId,
                                         @WarehouseRootId,@SCMName,@WarehouseRoot,@CreatedBy,@CreatedDate,1,1)", forecastingItemViewModel.ForecastingItemSummary, transaction: transaction, commandType: CommandType.Text);

                        connection.Execute(@"Insert INTO ForecastingItem 
                                                 (OrderId,ItemMasterID,SellerID,SKU,FNSKU,QuantitySold,TotalSold,AvgSoldPercentage,AvgSold,TotalQtyNeeded,
                                                 OrderByFNSKU,FinalOrder,QtyPerCarton,TotalCarton,BoxDim,CBM3,Total,CreatedBy,CreatedDate,IsActive,IsActiveSupplierOrder)
                                                 VALUES(@OrderId,@ItemMasterID,@SellerID,@SKU,@FNSKU,@QuantitySold,@TotalSold,
                                                 @AvgSoldPercentage,@AvgSold,@TotalQtyNeeded,@OrderByFNSKU,@FinalOrder,@QtyPerCarton,
                                                 @TotalCarton,@BoxDim,@CBM3,@Total,@CreatedBy,@CreatedDate,1,1)", forecastingItemViewModel.ForecastingItems, transaction: transaction, commandType: CommandType.Text);

                        transaction.Commit();
                        result = "1";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = "2";
                    }
                }
            }
            return result;
        }

        public string SaveSupplierOrder(SupplierOrderViewModel supplierOrderViewModel)
        {
            string result = "";
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(@"Insert INTO SupplierOrderSummary 
                                         (SupplierOrderId,SupplierName,WarehouseRoot,ShipmentTitle,TotalQty,TotalBox,TotalValue,TotalCVM,DepositPercentage,
                                         DepositAmount,Currency,GBPRate,Source,ETDDate,DepositDate,CutOffDate,PortOfLoading,PortOfArrival,IsActive,LeadTime,
                                         CreatedBy,CreatedDate,IsActiveSupplierOrder,IsActiveCreateShipment)
                                         VALUES(@SupplierOrderId,@SupplierName,@WarehouseRoot,@ShipmentTitle,@TotalQty,@TotalBox,@TotalValue,@TotalCVM,
                                         @DepositPercentage,@DepositAmount,@Currency,@GBPRate,@Source,@ETDDate,@DepositDate,@CutOffDate,@PortOfLoading,
                                         @PortOfArrival,@IsActive,@LeadTime,@CreatedBy,@CreatedDate,1,1)", supplierOrderViewModel.SupplierOrderSummary, transaction: transaction, commandType: CommandType.Text);

                        connection.Execute(@"Insert INTO SupplierOrder 
                                                 (OrderId,SupplierOrderId,ItemMasterId,ItemName,ItemCode,SupplierName,BatchNo,OrderDateString,TotalCVM,TotalQty,
                                                 TotalValue,TotalBox,PortOfLoading,WarehouseRoot,LeadTime,IsActive,CreatedBy,CreatedDate,IsActiveSupplierOrder)
                                                 VALUES(@OrderId,@SupplierOrderId,@ItemMasterId,@ItemName,@ItemCode,@SupplierName,@BatchNo,
                                                 @OrderDateString,@TotalCVM,@TotalQty,@TotalValue,@TotalBox,@PortOfLoading,
                                                 @WarehouseRoot,@LeadTime,@IsActive,@CreatedBy,@CreatedDate,1)", supplierOrderViewModel.SupplierOrders, transaction: transaction, commandType: CommandType.Text);

                        foreach (var data in supplierOrderViewModel.SupplierOrders)
                        {
                            var queryForecastingItemSummary = $"Update ForecastingItemSummary Set IsActiveSupplierOrder=0, IsActive=0 where OrderId=@OrderId";

                            var parametersForecastingItem = new DynamicParameters();
                            parametersForecastingItem.Add("@OrderId", data.OrderId);

                            int rowsAffected=connection.Execute(queryForecastingItemSummary, parametersForecastingItem, transaction: transaction, commandType: CommandType.Text);

                            if (rowsAffected > 0)
                            {
                                string queryForecastingItem = string.Format("UPDATE FI SET FI.IsActive=0 FROM ForecastingItem AS FI INNER JOIN ForecastingItemSummary AS FIS ON FI.OrderId = FIS.OrderId WHERE FIS.OrderId=@OrderId");
                                connection.Execute(queryForecastingItem, parametersForecastingItem, transaction: transaction, commandType: CommandType.Text);
                                
                            }
                        }

                        transaction.Commit();
                        result = "1";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = "2";
                    }
                }
            }
            return result;
        }

        public string SaveCreateShipment(CreateShipmentViewModel createShipmentViewModel)
        {
            string result = "";
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(@"Insert INTO CreateShipmentSummary 
                                         (ShipmentOrderId,ShipmentId,WarehouseRoot,ShipmentTitle,PortOfLoading,PortOfArrival,Currency,TotalValue,DepositAmount,PaidDeposit,ContainerSize,TotalCVM,ETDDate,ETADate,Comment,CaseWorker,
                                         FreightCharges, BookingAgent, ClearingAgent, ShippingAgent, RemainingBalance, IsActive, CreatedBy,CreatedDate)
                                         VALUES(@ShipmentOrderId,@ShipmentId,@WarehouseRoot,@ShipmentTitle,@PortOfLoading,@PortOfArrival,@Currency,@TotalValue,@DepositAmount,@PaidDeposit,@ContainerSize,@TotalCVM,@ETDDate,@ETADate,@Comment,@CaseWorker,
                                         @FreightCharges,@BookingAgent,@ClearingAgent,@ShippingAgent,@RemainingBalance,@IsActive,@CreatedBy,@CreatedDate)", createShipmentViewModel.CreateShipmentSummary, transaction: transaction, commandType: CommandType.Text);

                        connection.Execute(@"Insert INTO CreateShipment 
                                         (SupplierOrderId,ShipmentOrderId,SupplierName,Currency,TotalValue,DepositPercentage,DepositAmount,DepositDate,RemainingBalance,LeadTime,ETDDate,
                                         TotalCVM,PortOfLoading,PortOfArrival,WarehouseRoot,IsActive,CreatedBy,CreatedDate)
                                         VALUES(@SupplierOrderId,@ShipmentOrderId,@SupplierName,@Currency,@TotalValue,@DepositPercentage,@DepositAmount,@DepositDate,@RemainingBalance,@LeadTime,@ETDDate,
                                         @TotalCVM,@PortOfLoading,@PortOfArrival,@WarehouseRoot,@IsActive,@CreatedBy,@CreatedDate)", createShipmentViewModel.CreateShipments, transaction: transaction, commandType: CommandType.Text);

                        foreach (var data in createShipmentViewModel.CreateShipments)
                        {
                            var queryCreateShipment = $"Update SupplierOrderSummary Set IsActive=0 where SupplierOrderId=@SupplierOrderId";

                            var parametersCreateShipments = new DynamicParameters();
                            parametersCreateShipments.Add("@SupplierOrderId", data.SupplierOrderId);

                            connection.Execute(queryCreateShipment, parametersCreateShipments, transaction: transaction, commandType: CommandType.Text);
                        }

                        transaction.Commit();
                        result = "1";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = "2";
                    }
                }
            }
            return result;
        }

        public string UpdateItemForecasting(ForecastingItemViewModel forecastingItemViewModel)
        {
            string result = "";
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryForecastingItemSummary = $"Update ForecastingItemSummary Set SupplierName=@SupplierName,BatchNo=@BatchNo,OrderDate=@OrderDate,TotalCVM=@TotalCVM,SupplierCurrency=@SupplierCurrency,SupplierCost=@SupplierCost,TotalQty=@TotalQty,TotalValue=@TotalValue,TotalBox=@TotalBox,PortOfLoading=@PortOfLoading,Comments=@Comments,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate where Id=@Id;";

                        var parametersForecastingItemSummary = new DynamicParameters();
                        parametersForecastingItemSummary.Add("@SupplierName", forecastingItemViewModel.ForecastingItemSummary.SupplierName);
                        parametersForecastingItemSummary.Add("@BatchNo", forecastingItemViewModel.ForecastingItemSummary.BatchNo);
                        parametersForecastingItemSummary.Add("@OrderDate", forecastingItemViewModel.ForecastingItemSummary.OrderDate);
                        parametersForecastingItemSummary.Add("@TotalCVM", forecastingItemViewModel.ForecastingItemSummary.TotalCVM);
                        parametersForecastingItemSummary.Add("@SupplierCurrency", forecastingItemViewModel.ForecastingItemSummary.SupplierCurrency);
                        parametersForecastingItemSummary.Add("@SupplierCost", forecastingItemViewModel.ForecastingItemSummary.SupplierCost);
                        parametersForecastingItemSummary.Add("@TotalQty", forecastingItemViewModel.ForecastingItemSummary.TotalQty);
                        parametersForecastingItemSummary.Add("@TotalValue", forecastingItemViewModel.ForecastingItemSummary.TotalValue);
                        parametersForecastingItemSummary.Add("@TotalBox", forecastingItemViewModel.ForecastingItemSummary.TotalBox);
                        parametersForecastingItemSummary.Add("@PortOfLoading", forecastingItemViewModel.ForecastingItemSummary.PortOfLoading);
                        parametersForecastingItemSummary.Add("@Comments", forecastingItemViewModel.ForecastingItemSummary.Comments);
                        parametersForecastingItemSummary.Add("@UpdatedBy", forecastingItemViewModel.ForecastingItemSummary.UpdatedBy);
                        parametersForecastingItemSummary.Add("@UpdatedDate", forecastingItemViewModel.ForecastingItemSummary.UpdatedDate);
                        parametersForecastingItemSummary.Add("@Id", forecastingItemViewModel.ForecastingItemSummary.Id);

                        connection.Execute(queryForecastingItemSummary, parametersForecastingItemSummary, transaction: transaction, commandType: CommandType.Text);

                        string query = string.Format("Update ForecastingItem Set IsActive=0 where OrderId={0}", forecastingItemViewModel.ForecastingItemSummary.OrderId);
                        int rowsAffected = connection.Execute(query, transaction: transaction, commandType: CommandType.Text);

                        foreach (var data in forecastingItemViewModel.ForecastingItems)
                        {
                            var queryForecastingItem = $"Update ForecastingItem Set FinalOrder=@FinalOrder,TotalCarton=@TotalCarton,Total=@Total, IsActive=1 where Id=@Id;";

                            var parametersForecastingItem = new DynamicParameters();
                            parametersForecastingItem.Add("@FinalOrder", data.FinalOrder);
                            parametersForecastingItem.Add("@TotalCarton", data.TotalCarton);
                            parametersForecastingItem.Add("@Total", data.Total);
                            parametersForecastingItem.Add("@Id", data.Id);

                            connection.Execute(queryForecastingItem, parametersForecastingItem, transaction: transaction, commandType: CommandType.Text);
                        }

                        transaction.Commit();
                        result = "1";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = "2";
                    }
                }
            }
            return result;
        }

        public string UpdateCreateShipment(CreateShipmentViewModel createShipmentViewModel)
        {
            string result = "";
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var queryForecastingItemSummary = $"Update CreateShipmentSummary Set ShipmentId=@ShipmentId,ShipmentTitle=@ShipmentTitle,Currency=@Currency,PaidDeposit=@PaidDeposit,DepositAmount=@DepositAmount,ContainerSize=@ContainerSize,CaseWorker=@CaseWorker,ShippingAgent=@ShippingAgent,ClearingAgent=@ClearingAgent,FreightCharges=@FreightCharges,ETDDate=@ETDDate,ETADate=@ETADate,Comment=@Comment,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate where Id=@Id;";

                        var parametersForecastingItemSummary = new DynamicParameters();
                        parametersForecastingItemSummary.Add("@ShipmentId", createShipmentViewModel.CreateShipmentSummary.ShipmentId);
                        parametersForecastingItemSummary.Add("@ShipmentTitle", createShipmentViewModel.CreateShipmentSummary.ShipmentTitle);
                        parametersForecastingItemSummary.Add("@Currency", createShipmentViewModel.CreateShipmentSummary.Currency);
                        parametersForecastingItemSummary.Add("@PaidDeposit", createShipmentViewModel.CreateShipmentSummary.PaidDeposit);
                        parametersForecastingItemSummary.Add("@DepositAmount", createShipmentViewModel.CreateShipmentSummary.DepositAmount);
                        parametersForecastingItemSummary.Add("@ContainerSize", createShipmentViewModel.CreateShipmentSummary.ContainerSize);
                        parametersForecastingItemSummary.Add("@CaseWorker", createShipmentViewModel.CreateShipmentSummary.CaseWorker);
                        parametersForecastingItemSummary.Add("@ShippingAgent", createShipmentViewModel.CreateShipmentSummary.ShippingAgent);
                        parametersForecastingItemSummary.Add("@ClearingAgent", createShipmentViewModel.CreateShipmentSummary.ClearingAgent);
                        parametersForecastingItemSummary.Add("@FreightCharges", createShipmentViewModel.CreateShipmentSummary.FreightCharges);
                        parametersForecastingItemSummary.Add("@ETDDate", createShipmentViewModel.CreateShipmentSummary.ETDDate);
                        parametersForecastingItemSummary.Add("@ETADate", createShipmentViewModel.CreateShipmentSummary.ETADate);
                        parametersForecastingItemSummary.Add("@Comment", createShipmentViewModel.CreateShipmentSummary.Comment);
                        parametersForecastingItemSummary.Add("@UpdatedBy", createShipmentViewModel.CreateShipmentSummary.UpdatedBy);
                        parametersForecastingItemSummary.Add("@UpdatedDate", createShipmentViewModel.CreateShipmentSummary.UpdatedDate);
                        parametersForecastingItemSummary.Add("@Id", createShipmentViewModel.CreateShipmentSummary.Id);

                        connection.Execute(queryForecastingItemSummary, parametersForecastingItemSummary, transaction: transaction, commandType: CommandType.Text);

                        transaction.Commit();
                        result = "1";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = "2";
                    }
                }
            }
            return result;
        }

        public List<ForecastingItemSummary> GetForecastingItemsSummaryList()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<ForecastingItemSummary>("GetForecastingItemSummaryList", commandType: CommandType.StoredProcedure, commandTimeout: 0);
                List<ForecastingItemSummary> forecastingItemSummarieList = result.ToList();

                return forecastingItemSummarieList;
            }
        }

        public List<SupplierOrderSummary> GetSupplierOrderList()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<SupplierOrderSummary>("GetSupplierOrderList", commandType: CommandType.StoredProcedure, commandTimeout: 0);
                List<SupplierOrderSummary> supplierOrderList = result.ToList();

                return supplierOrderList;
            }
        }

        public List<CreateShipmentSummary> GetCreateShipmentList()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<CreateShipmentSummary>("GetCreateShipmentList", commandType: CommandType.StoredProcedure, commandTimeout: 0);
                List<CreateShipmentSummary> createShipmentList = result.ToList();

                return createShipmentList;
            }
        }

        public List<ShipmentViewModel> GetShipmentHistory(string id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            string query = string.Format(@"Select CSS.ShipmentOrderId,SOS.ShipmentTitle
                                            ,SOS.SupplierOrderId, FIS.OrderId
                                            ,FORMAT (FIS.OrderDate,'yyyy-MM-dd') as OrderDate
                                            ,IM.ItemName ,FIS.SupplierName ,FIS.TotalQty ,FIS.TotalBox
                                            ,FIS.SupplierCost ,FIS.TotalValue ,FIS.WarehouseRoot
                                            
                                            from CreateShipmentSummary as CSS
                                            inner join CreateShipment as CS on CSS.ShipmentOrderId=CS.ShipmentOrderId
                                            inner join SupplierOrderSummary as SOS on CS.SupplierOrderId=SOS.SupplierOrderId
                                            inner join SupplierOrder as SO on SOS.SupplierOrderId=SO.SupplierOrderId
                                            inner join ForecastingItemSummary as FIS on SO.OrderId=FIS.OrderId
                                            inner join ItemMasters as IM on FIS.ItemMasterId=IM.ItemMasterID
                                            Where CSS.Id = @id
                                           ORDER BY CSS.ShipmentOrderId  DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize);

            string countQuery = @"Select Count(*)
                                            from CreateShipmentSummary as CSS
                                            inner join CreateShipment as CS on CSS.ShipmentOrderId=CS.ShipmentOrderId
                                            inner join SupplierOrderSummary as SOS on CS.SupplierOrderId=SOS.SupplierOrderId
                                            inner join SupplierOrder as SO on SOS.SupplierOrderId=SO.SupplierOrderId
                                            inner join ForecastingItemSummary as FIS on SO.OrderId=FIS.OrderId
                                            inner join ItemMasters as IM on FIS.ItemMasterId=IM.ItemMasterID
                                            Where CSS.Id = @id;";
            rowCount = 0;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var reader = connection.QueryMultiple(countQuery + query, new { id = id });

                rowCount = reader.Read<int>().Single();

                var result = reader.Read<ShipmentViewModel>();

                return result.ToList();
            }
        }

        public List<SupplierOrderSummary> GetSupplierOrderListWithWarehouse(out int totalCount, string warehouseRoot = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            string query = "";
            string countQuery = "";

            if (warehouseRoot == "" )
            {
                query = string.Format(@"Select SOS.Id,SOS.SupplierOrderId,SOS.SupplierName,SOS.ShipmentTitle,SOS.Currency,
	                                    SOS.TotalValue,SOS.DepositPercentage,SOS.DepositAmount,FORMAT (SOS.DepositDate, 'yyyy-MM-dd') as DepositDateString,
	                                    (SOS.TotalValue-SOS.DepositAmount) as RemainingBalance,SOS.LeadTime,FORMAT (SOS.ETDDate, 'yyyy-MM-dd') as ETDDateString,
	                                    FORMAT (SOS.CutOffDate, 'yyyy-MM-dd') as CutOffDateString,SOS.TotalCVM,SOS.PortOfLoading,SOS.PortOfArrival,SOS.WarehouseRoot
	                                    from SupplierOrderSummary AS SOS
	                                    WHERE SOS.IsActive=1 Order By SOS.Id DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize);

                countQuery = string.Format(@"Select COUNT(*) from SupplierOrderSummary WHERE IsActive=1");
            }
            else
            {
                query = string.Format(@"Select SOS.Id,SOS.SupplierOrderId,SOS.SupplierName,SOS.ShipmentTitle,SOS.Currency,
	                                    SOS.TotalValue,SOS.DepositPercentage,SOS.DepositAmount,FORMAT (SOS.DepositDate, 'yyyy-MM-dd') as DepositDateString,
	                                    (SOS.TotalValue-SOS.DepositAmount) as RemainingBalance,SOS.LeadTime,FORMAT (SOS.ETDDate, 'yyyy-MM-dd') as ETDDateString,
	                                    FORMAT (SOS.CutOffDate, 'yyyy-MM-dd') as CutOffDateString,SOS.TotalCVM,SOS.PortOfLoading,SOS.PortOfArrival,SOS.WarehouseRoot
	                                    from SupplierOrderSummary AS SOS
	                                    WHERE SOS.IsActive=1 And SOS.WarehouseRoot='{0}' Order By SOS.Id DESC OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY;", warehouseRoot, jtStartIndex, jtPageSize);

                countQuery = string.Format(@"Select COUNT(*) from SupplierOrderSummary WHERE IsActive=1 And WarehouseRoot='{0}'", warehouseRoot);
            }


            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var parameters = new DynamicParameters();

                var reader = connection.QueryMultiple(countQuery + query, parameters);
                totalCount = reader.Read<int>().Single();
            }

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<SupplierOrderSummary>(query, commandType: CommandType.Text, commandTimeout: 0);

                List<SupplierOrderSummary> supplierOrderSummary = result.ToList();

                return supplierOrderSummary;
            }
        }

        public List<ForecastingItemSummary> GetsItemForecastingSummaryListForSupplierOrder(out int totalCount, string warehouseRoot = "", string supplierName = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            string query = "";
            string countQuery = "";

            if (warehouseRoot == "" && supplierName == "")
            {
                query = string.Format(@"Select FIS.Id, FIS.OrderId, IM.ItemMasterID, IM.ItemName, IM.ItemCode,
                                             FIS.SupplierName, FIS.TotalQty, FIS.TotalBox, FIS.SupplierCost,
                                             FIS.SupplierCurrency, FIS.TotalValue, FIS.TotalCVM, FORMAT (FIS.OrderDate, 'dd-MM-yyyy') as OrderDateString,
                                             FIS.PortOfLoading, FIS.SCMName, FIS.WarehouseRoot, FIS.LeadTime, FIS.Comments, FIS.BatchNo,
                                                FORMAT (FIS.CreatedDate, 'dd-MM-yyyy') as CreatedDateString,FIS.SupplierCurrency
                                             from ForecastingItemSummary AS FIS
                                             INNER JOIN ItemMasters AS IM ON FIS.ItemMasterId=IM.ItemMasterID
                                             WHERE FIS.IsActive=1 And FIS.IsActiveSupplierOrder=1 Order By FIS.Id DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize);

                countQuery = string.Format(@"Select COUNT(*)
                                             from ForecastingItemSummary AS FIS
                                             INNER JOIN ItemMasters AS IM ON FIS.ItemMasterId=IM.ItemMasterID
                                             WHERE FIS.IsActive=1 And FIS.IsActiveSupplierOrder=1");
            }
            else
            {
                query = string.Format(@"Select FIS.Id, FIS.OrderId, IM.ItemMasterID, IM.ItemName, IM.ItemCode,
                                             FIS.SupplierName, FIS.TotalQty, FIS.TotalBox, FIS.SupplierCost,
                                             FIS.SupplierCurrency, FIS.TotalValue, FIS.TotalCVM, FORMAT (FIS.OrderDate, 'dd-MM-yyyy') as OrderDateString,
                                             FIS.PortOfLoading, FIS.SCMName, FIS.WarehouseRoot, FIS.LeadTime, FIS.Comments, FIS.BatchNo,
                                                FORMAT (FIS.CreatedDate, 'dd-MM-yyyy') as CreatedDateString, FIS.SupplierCurrency
                                             from ForecastingItemSummary AS FIS
                                             INNER JOIN ItemMasters AS IM ON FIS.ItemMasterId=IM.ItemMasterID
                                             WHERE FIS.IsActive=1 And FIS.IsActiveSupplierOrder=1 And FIS.WarehouseRootId={0} and FIS.SupplierName='{1}' Order By FIS.Id DESC OFFSET {2} ROWS FETCH NEXT {3} ROWS ONLY;", warehouseRoot, supplierName, jtStartIndex, jtPageSize);

                countQuery = string.Format(@"Select COUNT(*)
                                             from ForecastingItemSummary AS FIS
                                             INNER JOIN ItemMasters AS IM ON FIS.ItemMasterId=IM.ItemMasterID
                                             WHERE FIS.IsActive=1 And FIS.IsActiveSupplierOrder=1 and FIS.WarehouseRootId={0} and FIS.SupplierName='{1}'", warehouseRoot, supplierName);
            }


            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var parameters = new DynamicParameters();

                var reader = connection.QueryMultiple(countQuery + query, parameters);
                totalCount = reader.Read<int>().Single();
            }

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<ForecastingItemSummary>(query, commandType: CommandType.Text, commandTimeout: 0);

                List<ForecastingItemSummary> forecastingItemSummarieList = result.ToList();

                return forecastingItemSummarieList;
            }
        }

        public bool RemoveForecastingItem(int id)
        {
            int rowsAffected = 0;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string queryForecastingItemSummary = string.Format("UPDATE ForecastingItemSummary SET IsActive=0 WHERE Id={0}", id);
                rowsAffected = connection.Execute(queryForecastingItemSummary, commandType: CommandType.Text);

                if (rowsAffected > 0)
                {
                    string queryForecastingItem = string.Format("UPDATE FI SET FI.IsActive=0 FROM ForecastingItem AS FI INNER JOIN ForecastingItemSummary AS FIS ON FI.OrderId = FIS.OrderId WHERE FIS.Id ={0}", id);
                    rowsAffected = connection.Execute(queryForecastingItem, commandType: CommandType.Text);
                }
                return rowsAffected > 0;
            }
        }

        public bool RemoveSupplierOrder(int id)
        {
            bool result = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var parametersSupplierOrderId = new DynamicParameters();
                        parametersSupplierOrderId.Add("@Id", id);

                        var querySupplierOrderSummary = $"UPDATE SupplierOrderSummary SET IsActive=0 WHERE Id=@Id";
                        connection.Execute(querySupplierOrderSummary, parametersSupplierOrderId, transaction: transaction, commandType: CommandType.Text);

                        string querySupplierOrder = string.Format("UPDATE SO SET SO.IsActive=0 FROM SupplierOrder AS SO INNER JOIN SupplierOrderSummary AS SOS ON SO.SupplierOrderId = SOS.SupplierOrderId WHERE SOS.Id =@Id");
                        connection.Execute(querySupplierOrder, parametersSupplierOrderId, transaction: transaction, commandType: CommandType.Text);

                        string queryForecastingItemSummary = string.Format("UPDATE ForecastingItemSummary SET ForecastingItemSummary.IsActive = 1 FROM ForecastingItemSummary as FIS inner join SupplierOrder as SO on FIS.OrderId = SO.OrderId Where SO.SupplierOrderId = @Id");
                        connection.Execute(queryForecastingItemSummary, parametersSupplierOrderId, transaction: transaction, commandType: CommandType.Text);

                        string queryForecastingItem = string.Format("UPDATE ForecastingItem SET ForecastingItem.IsActive=1 FROM ForecastingItem as FI inner join SupplierOrder as SO on FI.OrderId=SO.OrderId Where SO.SupplierOrderId = @Id");
                        connection.Execute(queryForecastingItem, parametersSupplierOrderId, transaction: transaction, commandType: CommandType.Text);

                        transaction.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = false;
                    }
                }
            }
            return result;
        }

        public bool RemoveCreateShipment(int id)
        {
            bool result = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var parameterCreateShipmentId = new DynamicParameters();
                        parameterCreateShipmentId.Add("@Id", id);

                        var queryCreateShipmentSummary = $"UPDATE CreateShipmentSummary SET IsActive=0 WHERE Id=@Id";
                        connection.Execute(queryCreateShipmentSummary, parameterCreateShipmentId, transaction: transaction, commandType: CommandType.Text);

                        string queryCreateShipment = string.Format("UPDATE CS SET CS.IsActive=0 FROM CreateShipment AS CS INNER JOIN CreateShipmentSummary AS CSS ON CS.ShipmentOrderId = CSS.ShipmentOrderId WHERE CSS.Id =@Id");
                        connection.Execute(queryCreateShipment, parameterCreateShipmentId, transaction: transaction, commandType: CommandType.Text);

                        string querySupplierOrderSummary = string.Format("UPDATE SupplierOrderSummary SET SupplierOrderSummary.IsActive=1 FROM SupplierOrderSummary as SOS inner join CreateShipment as CS on SOS.SupplierOrderId=CS.SupplierOrderId Where CS.ShipmentOrderId = @Id");
                        connection.Execute(querySupplierOrderSummary, parameterCreateShipmentId, transaction: transaction, commandType: CommandType.Text);

                        string querySupplierOrder = string.Format("UPDATE SupplierOrder SET SupplierOrder.IsActive=1 FROM SupplierOrder as SO inner join CreateShipment as CS on SO.SupplierOrderId=CS.SupplierOrderId Where CS.ShipmentOrderId = @Id");
                        connection.Execute(querySupplierOrder, parameterCreateShipmentId, transaction: transaction, commandType: CommandType.Text);

                        transaction.Commit();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result = false;
                    }
                }
            }
            return result;
        }

        public ForecastingItemSummary GetItemForecastingByID(int Id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select * from ForecastingItemSummary where IsActive=1 AND Id={0}",Id);

                IEnumerable<ForecastingItemSummary> forecastingItemSummary = connection.Query<ForecastingItemSummary>(query, commandType: CommandType.Text);

                var result = forecastingItemSummary.FirstOrDefault();

                return result;
            }
        }

        public SupplierOrderSummary GetSupplierOrderByID(int Id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select * from SupplierOrderSummary where IsActive=1 AND Id={0}", Id);

                IEnumerable<SupplierOrderSummary> supplierOrderSummary = connection.Query<SupplierOrderSummary>(query, commandType: CommandType.Text);

                var result = supplierOrderSummary.FirstOrDefault();

                return result;
            }
        }

        public CreateShipmentSummary GetCreateShipmentByID(int Id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select * from CreateShipmentSummary where IsActive=1 AND Id={0}", Id);

                IEnumerable<CreateShipmentSummary> createShipmentSummary = connection.Query<CreateShipmentSummary>(query, commandType: CommandType.Text);

                var result = createShipmentSummary.FirstOrDefault();

                return result;
            }
        }

        public List<ForecastingItemSummary> GetSupplierNameFromForecasting()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select Distinct SupplierName from ForecastingItemSummary where IsActive=1");

                IEnumerable<ForecastingItemSummary> forecastingItemSummary = connection.Query<ForecastingItemSummary>(query, commandType: CommandType.Text);

                var result = forecastingItemSummary.ToList();

                return result;
            }
        }

        public List<ForecastingItem> GetsItemForecastingListDataById(int? id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select FI.Id,FI.OrderId,FI.ItemMasterID,FI.SellerID,FI.SKU,FI.FNSKU,FI.QuantitySold,FI.TotalSold,FI.AvgSoldPercentage,FI.AvgSold,FI.TotalQtyNeeded, FI.OrderByFNSKU,FI. FinalOrder, FI.QtyPerCarton, FI.TotalCarton, FI.BoxDim, FI.CBM3, FI.Total, FI.CreatedBy, FI.CreatedDate, FI.IsActive from ForecastingItem as FI inner join ForecastingItemSummary as FIS ON FI.OrderId=FIS.OrderId Where FIS.Id={0} and FIS.IsActive=1", id);

                IEnumerable<ForecastingItem> forecastingItem = connection.Query<ForecastingItem>(query, commandType: CommandType.Text);

                var result = forecastingItem.ToList();

                return result;
            }
        }

        public List<SupplierOrders> GetSupplierOrderListById(int? id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select SO.Id,SO.SupplierOrderId,SO.OrderId,SO.ItemName,SO.ItemCode,SO.SupplierName,SO.TotalQty,SO.TotalBox,SO.TotalValue,SO.TotalCVM,SO.OrderDateString,SO.PortOfLoading,SO.BatchNo,SO.WarehouseRoot,SO.LeadTime from SupplierOrder as SO inner join SupplierOrderSummary as SOS ON SO.SupplierOrderId=SOS.SupplierOrderId Where SOS.Id={0} and SOS.IsActive=1", id);

                IEnumerable<SupplierOrders> supplierOrders = connection.Query<SupplierOrders>(query, commandType: CommandType.Text);

                var result = supplierOrders.ToList();

                return result;
            }
        }

        public List<CreateShipment> GetCreateShipmentListById(int? id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select CS.Id,CS.ShipmentOrderId,CS.SupplierName,CS.Currency,CS.TotalValue,CS.DepositPercentage,CS.DepositAmount,FORMAT (CS.DepositDate, 'dd-MM-yyyy') as DepositDateString,CS.RemainingBalance,CS.LeadTime,FORMAT (CS.ETDDate, 'dd-MM-yyyy') as ETDDateString, FORMAT (CS.CutOffDate, 'dd-MM-yyyy') as CutOffDateString,CS.TotalCVM, CS.PortOfLoading, CS.PortOfArrival, CS.WarehouseRoot from CreateShipment as CS inner join CreateShipmentSummary as CSS ON CS.ShipmentOrderId=CSS.ShipmentOrderId Where CSS.Id={0} and CSS.IsActive=1", id);

                IEnumerable<CreateShipment> createShipments = connection.Query<CreateShipment>(query, commandType: CommandType.Text);

                var result = createShipments.ToList();

                return result;
            }
        }
    }
}
