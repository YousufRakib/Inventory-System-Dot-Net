using Infrastructure.ConfigurationProvider;
using Infrastructure.Core.DataAccess;
using Infrastrucutre.Core.Models;
using Dapper;
using DapperExtensions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using DapperExtensions.Mapper;
using Infrastrucutre.Core.Models.ViewModels;
using System.Reflection;
using Infrastrucutre.Core.Models.Models;

namespace Infrastrucutre.Core.DataAccess
{



    public class ItemRepository : IItemRepository
    {
        //Removing Entity Framework references
        //RepositoryContext db = new RepositoryContext();

        public StockOut AddStockOut(StockOut option)
        {
            var record = new StockOut();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                int id = connection.Insert<StockOut>(option);

                record = connection.Get<StockOut>(id);
            }

            return record;

        }

        public bool UpdateStockOut(StockOut option)
        {

            bool updated = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updated = connection.Update<StockOut>(option);
            }

            return updated;
        }

        public List<AutoCompleteItem> GetSupplierNames(string supplierName = "")
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select SupplierName as label,SupplierID as id  from Suppliers WHERE SupplierName LIKE '{0}%'", supplierName);
                IEnumerable<AutoCompleteItem> items = connection.Query<AutoCompleteItem>(query, parameters, commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public List<AutoCompleteItem> GetItemNames(string itemName = "", bool onlyItemName = false)
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select TOP 10 (ItemName + '-' + ItemCode) as label,ItemMasterID as id from ItemMasters Where IsActive=1  AND itemName LIKE '{0}%'", itemName);
                if (onlyItemName)
                {
                    query = string.Format("Select TOP 10 ItemName as label,ItemMasterID as id from ItemMasters Where IsActive=1  AND itemName LIKE '{0}%'", itemName);
                }
                IEnumerable<AutoCompleteItem> items = connection.Query<AutoCompleteItem>(query, parameters, commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public StockSourceList GetStockSource() //8ugkcfcy
        {
            string query = @"Select SupplierID as [Id],SupplierName as [Value],'IN' As [Type] from Suppliers
                            UNION ALL
                            Select Id,Source,'OUT' from StockOut";

            var source = new StockSourceList();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {

                List<StockSource> items = connection.Query<StockSource>(query).ToList();

                source.Purchase = items.Where(i => i.Type == "IN").ToList();
                source.Sale = items.Where(i => i.Type == "OUT").ToList();

            }

            return source;
        }

        public StockView UpdateStock(ItemStock stock)
        {
            int updated;

            var view = new StockView();

            view.IsUpdated = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                stock.IsActive = true;

                updated = connection.Insert<ItemStock>(stock);

                var predicate = Predicates.Field<StockView>(f => f.ItemMasterID, Operator.Eq, stock.ItemMasterID);

                view = connection.GetList<StockView>(predicate).FirstOrDefault();

                view.IsUpdated = true;

            }

            return view;
        }

        //public StockView UpdateStock(ItemStock stock)
        //{
        //    int updated;

        //    var view = new StockViewForStockManagement();

        //    view.IsUpdated = false;

        //    using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
        //    {
        //        stock.IsActive = true;

        //        updated = connection.Insert<ItemStock>(stock);

        //        var predicate = Predicates.Field<StockViewForStockManagement>(f => f.ItemMasterID, Operator.Eq, stock.ItemMasterID);

        //        view = connection.GetList<StockViewForStockManagement>(predicate).FirstOrDefault();

        //        view.IsUpdated = true;

        //    }

        //    return view;
        //}
        public StockViewForStockManagement UpdateStockReturnStockManagementView(ItemStock stock)
        {
            int updated;

            var view = new StockViewForStockManagement();

            view.IsUpdated = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string queryItemCost = string.Format(@"SELECT * from ItemPrices where ItemMasterId  = {0} And FBARootID = {1} and IsItActive = 1;", stock.ItemMasterID, stock.FBARootID);
                var readerItemCost = connection.QueryMultiple(queryItemCost);
                var resultItemCost = readerItemCost.Read<ItemPrice>();

                if (resultItemCost.Count() > 0)
                {
                    stock.Vat = resultItemCost.FirstOrDefault().Vat;
                    stock.OriginalPrice = resultItemCost.FirstOrDefault().OriginalPrice;
                    stock.Price = resultItemCost.FirstOrDefault().Price; 
                }
                else {
                    stock.Vat = 0;
                    stock.OriginalPrice = 0;
                    stock.Price = 0;
                }
                stock.IsActive = true;
                updated = connection.Insert<ItemStock>(stock);

                //var predicate = Predicates.Field<StockViewForStockManagement>(f => f.ItemMasterID, Operator.Eq, stock.ItemMasterID);

                //view = connection.GetList<StockViewForStockManagement>(predicate).FirstOrDefault();

                //view.IsUpdated = true;

                string queryStock = string.Format(@"SELECT * from StockViewForStockManagements where ItemMasterId ={0} And FBARootID={1};", string.Join(",", stock.ItemMasterID), stock.FBARootID);
                var readerStockView = connection.QueryMultiple(queryStock);
                view = readerStockView.Read<StockViewForStockManagement>().FirstOrDefault();
                view.IsUpdated = true;
            }

            return view;
        }

        public List<ItemStockHistory> GetStockHistory(int id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            string query = string.Format(@"Select stock.*,supplier.SupplierName,source.Source ,master.ItemName,master.ItemCost, fbaRoot.FBARoot as FBARootName  from ItemStock stock 
                             inner join ItemMasters master on stock.ItemMasterID = master.ItemMasterID
                            left join Suppliers supplier on supplier.SupplierID = stock.InSource
                            left join StockOut source on  source.Id = stock.OutSource
                            left join FBARoot fbaRoot on  stock.FBARootId = fbaRoot.FBARootID 
                            WHERE stock.ItemMasterID = @id And stock.Type in(1,2)
                            ORDER BY StockId DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize);

            string countQuery = @"Select count(*) from ItemStock stock 
                                  inner join ItemMasters master on stock.ItemMasterID = master.ItemMasterID
                                  WHERE stock.ItemMasterID = @id;";
            rowCount = 0;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var reader = connection.QueryMultiple(countQuery + query, new { id = id });

                rowCount = reader.Read<int>().Single();

                var result = reader.Read<ItemStockHistory>();

                return result.ToList();
            }
        }

        public List<FDBStockUploadHistory> GetFDBStockUploadHistory(int id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            string query = string.Format(@"Select SF.Fnsku as FNSKU, SIL.ItemMasterID, SF.CreatedDate,SF.UpdatedDate, SF.Location AS FBARootName, SF.Seller as FBASellerName from FDBStockFile as SF
                                           INNER JOIN SellerItemLink as SIL on SF.Sku=SIL.SKU
                                           Where SIL.ItemMasterID=@id --AND SF.Location='UK' AND SF.Seller='ARSUK'
                                           ORDER BY SF.Seller ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize);

            string countQuery = @"Select COUNT(*) FBASellerName from FDBStockFile as SF
                                  INNER JOIN SellerItemLink as SIL on SF.Sku=SIL.SKU
                                  Where SIL.ItemMasterID = @id;";
            rowCount = 0;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var reader = connection.QueryMultiple(countQuery + query, new { id = id });

                rowCount = reader.Read<int>().Single();

                var result = reader.Read<FDBStockUploadHistory>();

                return result.ToList();
            }
        }

        public List<ItemMaster> GetItems(string itemName = "")
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ItemName", itemName, DbType.String, ParameterDirection.Input);

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.Item_GetAllItems";
                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }


        public List<StockOut> GetStockOut()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var list = connection.GetList<StockOut>();

                return list.ToList();
            }

        }

        public List<ItemMaster> GetItemsWithStock(out int totalCount, string itemName = "", string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            var parameters = new DynamicParameters();

            string whereClause = string.Empty;

            bool isStockQuery = false;

            itemName = string.IsNullOrWhiteSpace(itemName) ? "" : itemName.Trim();
            filter = string.IsNullOrWhiteSpace(filter) ? "" : filter.Trim();
            filterText = string.IsNullOrWhiteSpace(filterText) ? "" : filterText.Trim();

            if (filter != "" || itemName != "")
            {
                whereClause = " WHERE ";
                if (filter == "ItemName" || itemName != "")
                {
                    itemName = itemName == "" ? filterText : itemName;
                    whereClause += "IM.ItemName LIKE '%'+ @ItemName +'%'";
                    parameters.Add("@ItemName", itemName, DbType.String, ParameterDirection.Input);
                }

                if (filter == "ItemCode")
                {
                    whereClause += "IM.ItemCode LIKE '%'+ @ItemCode +'%'";
                    parameters.Add("@ItemCode", filterText, DbType.String, ParameterDirection.Input);
                }

                if (filter == "Supplier")
                {
                    whereClause += "S.SupplierName LIKE '%'+ @SupplierName +'%'";
                    parameters.Add("@SupplierName", filterText, DbType.String, ParameterDirection.Input);
                }

                int stock;

                if (filter == "AvaliableStockGreater")
                {
                    if (int.TryParse(filterText, out stock))
                    {
                        whereClause += "ISNULL(stock.CurrentStock,0) >=" + filterText;
                    }
                    else
                    {
                        whereClause += "ISNULL(stock.CurrentStock,0) >=0";
                    }

                    isStockQuery = true;
                }

                if (filter == "AvaliableStockLesser")
                {
                    if (int.TryParse(filterText, out stock))
                    {
                        whereClause += "ISNULL(stock.CurrentStock,0) <=" + filterText;
                    }
                    else
                    {
                        whereClause += "ISNULL(stock.CurrentStock,0) <=0";
                    }
                    isStockQuery = true;
                }

                if (filter == "LowStock")
                {
                    whereClause += "ISNULL(stock.CurrentStock,0) <= IM.ReOrderLevel";
                    isStockQuery = true;
                }
            }


            string countQuery = string.Format(@"SELECT COUNT(*) FROM 
		                        ItemMasters IM 
                                INNER JOIN ItemCategories IC on IC.ItemCategoryID = IM.ItemCategoryID AND IM.IsActive=1
		                        INNER JOIN ItemColors ICO on ICO.ItemColorID = IM.ItemColorID
		                        INNER JOIN ItemManufacturers IMF on IMF.ItemManufacturerID = IM.ItemManufacturerID
                                INNER JOIN Suppliers S on S.SupplierID = IM.SupplierID                                
                                {0}", whereClause);

            if (isStockQuery)
            {
                countQuery = string.Format(@"SELECT COUNT(*) FROM 
		                        ItemMasters IM 
                                INNER JOIN ItemCategories IC on IC.ItemCategoryID = IM.ItemCategoryID AND IM.IsActive=1
		                        INNER JOIN ItemColors ICO on ICO.ItemColorID = IM.ItemColorID
		                        INNER JOIN ItemManufacturers IMF on IMF.ItemManufacturerID = IM.ItemManufacturerID
                                INNER JOIN Suppliers S on S.SupplierID = IM.SupplierID
                                LEFT JOIN StockView stock on stock.ItemMasterID = IM.ItemMasterID
                                {0};", whereClause);
            }
            //,stock.StockIn,stock.StockOut,stock.CurrentStock
            string query = string.Format(@"SELECT IM.ItemMasterID,ItemName, ItemCode, [Description], 
                            Brand, Dimension, ItemWeight,IM.ItemCost , IM.VAT, 
                            TotalCost, BarCode, IM.SupplierID, IM.ItemCategoryID, IM.ItemManufacturerID, IM.ItemColorID,
		                    IC.CategoryName,ICO.Color,IMF.ManufacturerName,S.SupplierName,IM.StockUnits,IM.ReOrderLevel,
		                    ActiveListingCount.ListingCount as ActiveListings

		                    FROM 
		                    ItemMasters IM 
							--INNER JOIN ItemStock I on I.ItemMasterID = IM.ItemMasterID
		                    INNER JOIN ItemCategories IC on IC.ItemCategoryID = IM.ItemCategoryID AND IM.IsActive=1
		                    INNER JOIN ItemColors ICO on ICO.ItemColorID = IM.ItemColorID
		                    INNER JOIN ItemManufacturers IMF on IMF.ItemManufacturerID = IM.ItemManufacturerID
		                    INNER JOIN Suppliers S on S.SupplierID = IM.SupplierID
		                    LEFT JOIN vw_ListingLinkCount ActiveListingCount on ActiveListingCount.ItemMasterID = IM.ItemMasterID                          
                            
                            {2}
                            ORDER BY IM.ItemMasterID DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize, whereClause);
            //LEFT JOIN StockView stock on stock.ItemMasterID = IM.ItemMasterID



            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {

                var reader = connection.QueryMultiple(countQuery + query, parameters);
                totalCount = reader.Read<int>().Single();
                var result = reader.Read<ItemMaster>();

                List<int> lstItemMasterId = result.Select(x => x.ItemMasterID).Distinct().ToList();
                //try {
                //    string queryStock1 = string.Format(@"SELECT * from StockViewForStockManagements where ItemMasterId in ({0});", string.Join(",", lstItemMasterId));
                //    var readerStockView2 = connection.QueryMultiple(queryStock1);
                //    var resultStockView3 = readerStockView2.Read<StockViewForStockManagement>();
                //}
                //catch (Exception ex)
                //{ 
                //}
                string queryStock = string.Format(@"SELECT * from StockViewForStockManagements where ItemMasterId in ({0});", string.Join(",", lstItemMasterId));
                var readerStockView = connection.QueryMultiple(queryStock);
                var resultStockView = readerStockView.Read<StockViewForStockManagement>();

                //string queryItemCost = string.Format(@"SELECT * from ItemMasters where ItemMasterId in ({0});", string.Join(",", lstItemMasterId));
                //var readerItemCost = connection.QueryMultiple(queryItemCost);
                //var resultItemCost = readerItemCost.Read<ItemMaster>();
                string queryItemCost = string.Format(@"SELECT * from ItemPrices where ItemMasterId in ({0}) and IsItActive = 1;", string.Join(",", lstItemMasterId));
                var readerItemCost = connection.QueryMultiple(queryItemCost);
                var resultItemCost = readerItemCost.Read<ItemPrice>();

                string queryFBAStock = string.Format(@"select distinct SIL.ItemMasterID,SIL.SKU,FDBSF.Location,FDBSF.Seller, FDBSF.AfnTotalQuantity from SellerItemLink as SIL
                                                    inner join FDBStockFile as FDBSF on SIL.SKU=FDBSF.Sku
                                                    where SIL.ItemMasterID in ({0}) 
                                                    AND FDBSF.Sku is not null", string.Join(",", lstItemMasterId));

                var readerFBAStock = connection.QueryMultiple(queryFBAStock);
                var resultFBAStock = readerFBAStock.Read<FDBStockQueryResult>();


                foreach (var item in result)
                {
                    int ukWearHouseId = 1;
                    item.CostUk = GetPrice(item, resultItemCost, ukWearHouseId); //
                    item.UkWhStock = GetWHStock(resultStockView, ukWearHouseId, item.ItemMasterID);
                    item.UKFBAStock = GetFBAStock(resultFBAStock, "ARSUK", "UK", item.ItemMasterID);
                    item.NEEZUKFBAStock = GetFBAStock(resultFBAStock, "NEEZ", "UK", item.ItemMasterID);
                    //item.UKTotalStock = GetTotalStock(resultStockView, ukWearHouseId);
                    item.UKTotalStock = GetTotalStock(item.UkWhStock, item.UKFBAStock, item.NEEZUKFBAStock);
                    item.UKStockLife = "0";
                    item.UKReOrderQTY = "0";

                    int euWearHouseId = 2;
                    item.CostEU = GetPrice(item, resultItemCost, euWearHouseId);//resultItemCost.Where(x => x.ItemMasterID == item.ItemMasterID).FirstOrDefault().ItemCost.ToString();
                    item.EUWhStock = GetWHStock(resultStockView, euWearHouseId, item.ItemMasterID);
                    item.EUFBAStock = GetFBAStock(resultFBAStock, "ARSUK", "EU", item.ItemMasterID);
                    item.NEEZEUFBAStock = GetFBAStock(resultFBAStock, "NEEZ", "EU", item.ItemMasterID);
                    item.CDisFBCStock = "0";
                    item.EUTotalStock = GetTotalStock(item.EUWhStock, item.EUFBAStock, item.NEEZEUFBAStock);//GetTotalStock(resultStockView, euWearHouseId);
                    item.EUStockLevel = "0";
                    item.EUStockLife = "0";

                    int usaWearHouseId = 3;
                    item.CostUSA = GetPrice(item, resultItemCost, usaWearHouseId); //resultItemCost.Where(x => x.ItemMasterID == item.ItemMasterID).FirstOrDefault().ItemCost.ToString();
                    item.USAWhStock = GetWHStock(resultStockView, usaWearHouseId, item.ItemMasterID);
                    item.USAFBAStock = GetFBAStock(resultFBAStock, "ARSUK", "US", item.ItemMasterID);
                    item.NEEZUSAFBAStock = GetFBAStock(resultFBAStock, "NEEZ", "US", item.ItemMasterID);
                    item.USATotalStock = GetTotalStock(item.USAWhStock, item.USAFBAStock, item.NEEZUSAFBAStock);//GetTotalStock(resultStockView, usaWearHouseId);
                    item.USAStockLevel = "0";
                    item.USAStockLife = "0";

                    int caWearHouseId = 5;
                    item.CostCA = GetPrice(item, resultItemCost, caWearHouseId);//resultItemCost.Where(x => x.ItemMasterID == item.ItemMasterID).FirstOrDefault().ItemCost.ToString();
                    item.CAWhStock = GetWHStock(resultStockView, caWearHouseId, item.ItemMasterID);
                    item.CAFBAStock = GetFBAStock(resultFBAStock, "ARSUK", "CA", item.ItemMasterID);
                    item.NEEZCAFBAStock = GetFBAStock(resultFBAStock, "NEEZ", "CA", item.ItemMasterID);
                    item.CATotalStock = GetTotalStock(item.CAWhStock, item.CAFBAStock, item.NEEZCAFBAStock);//GetTotalStock(resultStockView, caWearHouseId);
                    item.CAStockLevel = "0";
                    item.CAStockLife = "0";

                    int auWearHouseId = 4;
                    item.CostAU = GetPrice(item, resultItemCost, auWearHouseId);//resultItemCost.Where(x => x.ItemMasterID == item.ItemMasterID).FirstOrDefault().ItemCost.ToString();
                    item.AUWhStock = GetWHStock(resultStockView, auWearHouseId, item.ItemMasterID);
                    item.AUFBAStock = GetFBAStock(resultFBAStock, "ARSUK", "AU", item.ItemMasterID);
                    item.NEEZAUFBAStock = GetFBAStock(resultFBAStock, "NEEZ", "AU", item.ItemMasterID);
                    item.AUTotalStock = GetTotalStock(item.AUWhStock, item.AUFBAStock, item.NEEZAUFBAStock);//GetTotalStock(resultStockView, auWearHouseId);
                    item.AUStockLevel = "0";
                    item.AUStockLife = "0";
                }

                return result.ToList();
            }
        }

        private string GetPrice(ItemMaster item, IEnumerable<ItemPrice> resultItemCost, int ukWearHouseId)
        {
            var result = resultItemCost.Where(x => x.ItemMasterID == item.ItemMasterID && x.FBARootId == ukWearHouseId).FirstOrDefault() ;
            if (result != null)
            {
                return result.Price.ToString(); ;
            }
            else {
                return "0";
            }
        }

        private string GetTotalStock(string WhStock, string FBAStock, string NEEZStock)
        {
            int whStock = !string.IsNullOrEmpty(WhStock) ? int.Parse(WhStock) : 0;
            int fbaStock = !string.IsNullOrEmpty(FBAStock) ? int.Parse(FBAStock) : 0;
            int neezStock = !string.IsNullOrEmpty(NEEZStock) ? int.Parse(NEEZStock) : 0;

            return (whStock + fbaStock + neezStock).ToString();
        }

        private string GetTotalStock(IEnumerable<StockViewForStockManagement> resultStockView, int wearHouseId)
        {
            var res = resultStockView.Where(x => x.FBARootID == wearHouseId).FirstOrDefault();
            if (res != null)
            {
                return (res.StockIn + res.StockOut).ToString();
            }
            else
            {
                return "0";
            }
        }
        //private string GetTotalStock(IEnumerable<StockViewForStockManagement> resultStockView, int wearHouseId)
        //{
        //    var res = resultStockView.Where(x => x.FBARootID == wearHouseId).FirstOrDefault();
        //    if (res != null)
        //    {
        //        return (res.StockIn + res.StockOut).ToString();
        //    }
        //    else
        //    {
        //        return "0";
        //    }
        //}

        private string GetFBAStock(IEnumerable<FDBStockQueryResult> resultStockView, string seller, string wearHouse, int itemMasterID)
        {
            var result = resultStockView.Where(x => x.Location == wearHouse && x.ItemMasterID == itemMasterID && x.Seller == seller).Select(x => x.AfnTotalQuantity).ToList().Distinct();
            if (result != null)
            {
                return result.Sum().ToString();
            }
            else
            {
                return "0";
            }
        }

        private string GetWHStock(IEnumerable<StockViewForStockManagement> resultStockView, int wearHouseId, int itemMasterID)
        {
            var res = resultStockView.Where(x => x.FBARootID == wearHouseId && x.ItemMasterID == itemMasterID).FirstOrDefault();
            if (res != null)
            {
                return (res.StockIn - res.StockOut).ToString();
            }
            else {
                return "0";
            }
        }

        public VMStockQuantityAndCost GetStockQuantityByItemMasterIdFBARootIdItemCode(VMSearchClass searchClass)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            { 
                string queryStock = string.Format(@"SELECT * from StockViewForStockManagements where ItemMasterId ={0} And FBARootID={1};", string.Join(",", searchClass.ItemMasterId), searchClass.FBARootID);
                var readerStockView = connection.QueryMultiple(queryStock);
                var resultStockView = readerStockView.Read<StockView>();

                string queryItemCost = string.Format(@"SELECT * from ItemPrices where ItemMasterId  = {0} and IsItActive = 1;", searchClass.ItemMasterId);
                var readerItemCost = connection.QueryMultiple(queryItemCost);
                var resultItemCost = readerItemCost.Read<ItemPrice>();

                //string queryItemCost = string.Format(@"SELECT * from ItemMasters where ItemMasterId ={0};", string.Join(",", searchClass.ItemMasterId));
                //var readerItemCost = connection.QueryMultiple(queryItemCost);
                //var resultItemCost = readerItemCost.Read<ItemMaster>();

                VMStockQuantityAndCost VMStockQuantityAndCost = new VMStockQuantityAndCost();
                if (resultItemCost.FirstOrDefault() != null)
                {
                    VMStockQuantityAndCost.ItemCost = resultItemCost.FirstOrDefault().Price;
                }
                else {
                    VMStockQuantityAndCost.ItemCost = 0;
                }


                if (resultStockView.Count() > 0)
                {
                    VMStockQuantityAndCost.StockQuantity = resultStockView.FirstOrDefault().StockIn - resultStockView.FirstOrDefault().StockOut;
                }

                return VMStockQuantityAndCost;
            }
        }


        public List<ItemHistory> GetItemHistory(int id)
        {
            var list = new List<ItemHistory>();

            string query = @"Select  IM.*,IC.CategoryName,S.SupplierName,IMF.ManufacturerName,ICO.Color,Users.UserName from ItemMaster_History IM
                            INNER JOIN ItemCategories IC on IC.ItemCategoryID = IM.ItemCategoryID 
                            INNER JOIN ItemColors ICO on ICO.ItemColorID = IM.ItemColorID
                            INNER JOIN ItemManufacturers IMF on IMF.ItemManufacturerID = IM.ItemManufacturerID
                            INNER JOIN Suppliers S on S.SupplierID = IM.SupplierID
                            LEFT JOIN AppUsers Users on Users.UserID = IM.ModifiedByUser
                            WHERE IM.ItemMasterID=@id
                            ORDER BY IM.ItemMasterHistoryID DESC";

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                list = connection.Query<ItemHistory>(query, new { id }).ToList();
            }

            return list;

        }


        public List<ItemMaster> GetItems(out int totalCount, string itemName = "", string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            var parameters = new DynamicParameters();

            string whereClause = string.Empty;


            itemName = string.IsNullOrWhiteSpace(itemName) ? "" : itemName.Trim();
            filter = string.IsNullOrWhiteSpace(filter) ? "" : filter.Trim();
            filterText = string.IsNullOrWhiteSpace(filterText) ? "" : filterText.Trim();

            if (filter != "" || itemName != "")
            {
                whereClause = " WHERE ";
                if (filter == "ItemName" || itemName != "")
                {
                    itemName = itemName == "" ? filterText : itemName;
                    whereClause += "IM.ItemName LIKE '%' + @ItemName + '%'";
                    parameters.Add("@ItemName", itemName, DbType.String, ParameterDirection.Input);
                }

                if (filter == "ItemCode")
                {
                    whereClause += "IM.ItemCode LIKE '%' + @ItemCode + '%'";
                    parameters.Add("@ItemCode", filterText, DbType.String, ParameterDirection.Input);
                }

                if (filter == "Notes")
                {
                    whereClause += "IM.Notes LIKE '%' + @Notes + '%'";
                    parameters.Add("@Notes", filterText, DbType.String, ParameterDirection.Input);
                }

                if (filter == "Supplier")
                {
                    whereClause += "S.SupplierName LIKE '%' + @SupplierName + '%'";
                    parameters.Add("@SupplierName", filterText, DbType.String, ParameterDirection.Input);
                }

                if (filter == "ListingCount")
                {
                    int count = 0;

                    if (int.TryParse(filterText.Trim(), out count))
                    {
                        whereClause += "ActiveListingCount.ListingCount>=@listingcount";
                        parameters.Add("@listingcount", count, DbType.Int32, ParameterDirection.Input);
                    }
                }

            }


            string countQuery = string.Format(@"SELECT COUNT(*) FROM 
		                        ItemMasters IM 
		                        INNER JOIN ItemCategories IC on IC.ItemCategoryID = IM.ItemCategoryID AND IM.IsActive=1
		                        INNER JOIN ItemColors ICO on ICO.ItemColorID = IM.ItemColorID
		                        INNER JOIN ItemManufacturers IMF on IMF.ItemManufacturerID = IM.ItemManufacturerID
                                INNER JOIN Suppliers S on S.SupplierID = IM.SupplierID
                                LEFT JOIN vw_ListingLinkCount ActiveListingCount on ActiveListingCount.ItemMasterID = IM.ItemMasterID
                                {0}", whereClause);

            string query = string.Format(@"SELECT IM.ItemMasterID,ItemName, ItemCode, [Description], 
                            Brand, Dimension, ItemWeight,IM.ItemCost,VAT, 
		                    TotalCost, BarCode, IM.SupplierID, IM.ItemCategoryID, IM.ItemManufacturerID, IM.ItemColorID,
		                    IC.CategoryName,ICO.Color,IMF.ManufacturerName,S.SupplierName,IM.StockUnits,IM.ReOrderLevel,
		                    ActiveListingCount.ListingCount AS ActiveListings
		                    FROM 
		                    ItemMasters IM 
		                    INNER JOIN ItemCategories IC on IC.ItemCategoryID = IM.ItemCategoryID AND IM.IsActive=1
		                    INNER JOIN ItemColors ICO on ICO.ItemColorID = IM.ItemColorID
		                    INNER JOIN ItemManufacturers IMF on IMF.ItemManufacturerID = IM.ItemManufacturerID
		                    INNER JOIN Suppliers S on S.SupplierID = IM.SupplierID
		                    LEFT JOIN vw_ListingLinkCount ActiveListingCount on ActiveListingCount.ItemMasterID = IM.ItemMasterID
                            {2}
                            ORDER BY IM.ItemMasterID DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize, whereClause);




            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {

                var reader = connection.QueryMultiple(countQuery + "\n" + query, parameters);

                totalCount = reader.Read<int>().Single();

                var result = reader.Read<ItemMaster>();

                return result.ToList();
            }

        }
        public List<ItemMaster> GetItemMasterList()
        {
            var itemMasters = new List<ItemMaster>();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                //string query = "Select * from ItemMasters";
                string query = "Select * from ItemMasters where isactive = 1";

                itemMasters = connection.Query<ItemMaster>(query, new { }).ToList();
            }

            return itemMasters;
        }

        public bool DeleteItem(int itemMasterID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Update ItemMasters Set IsActive=0 where ItemMasterID={0}", itemMasterID);
                int rowsAffected = connection.Execute(query, commandType: CommandType.Text);
                return rowsAffected > 0;
            }

        }

        public List<ItemMaster> GetItemNames()
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string query = "Select ItemName,ItemMasterID from ItemMasters Where IsActive=1";
                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(query, parameters, commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public string GetItemNameByItemMasterId(string itemMasterId)
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Select * from ItemMasters Where ItemMasterID={0}", itemMasterId);
                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(query, parameters, commandType: CommandType.Text);
                return items.Select(x=>x.ItemName).FirstOrDefault();
            }
        }

        public List<PostalCarrier> GetPostage() // Postage Carrier Name Load By D 15-02-2021
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string query = "Select PostalCarrierName,PostalCarrierID from PostalCarriers";
                IEnumerable<PostalCarrier> items = connection.Query<PostalCarrier>(query, parameters, commandType: CommandType.Text);
                return items.ToList();
            }
        }

        public List<PostalCarrier> GetPostageList() // Postage Carrier Name Load By D 15-02-2021
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string query = "Select * from PostalCarriers";
                IEnumerable<PostalCarrier> items = connection.Query<PostalCarrier>(query, parameters, commandType: CommandType.Text);
                return items.ToList();
            }
        }
        public bool AddItems(ItemMaster item) //-----
        {

            var parameters = new DynamicParameters();

            parameters.Add("@ItemName", item.ItemName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemCode", item.ItemCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemDescription", item.Description, DbType.String, ParameterDirection.Input);
            parameters.Add("@Brand", item.Brand, DbType.String, ParameterDirection.Input);
            parameters.Add("@Dimension", item.Dimension, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemWeight", item.ItemWeight, DbType.String, ParameterDirection.Input);
            parameters.Add("@VAT", item.VAT, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemCost", item.ItemCost, DbType.String, ParameterDirection.Input);
            parameters.Add("@BarCode", item.BarCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@SupplierID", item.SupplierID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemCategoryID", item.ItemCategoryID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemManufacturerID", item.ItemManufacturerID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemColorID", item.ItemColorID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@LocationID", item.LocationID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@StockUnits", 0, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ReOrderLevel", item.ReOrderLevel, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@MasterCartonQTY", item.MasterCartonQty, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CartonQty", item.CartonQty, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@LeadTime", item.LeadTime, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FNSKU", item.FNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@Notes", item.Notes, DbType.String, ParameterDirection.Input);
            parameters.Add("@DateOfSubmission", item.DateOfSubmission, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Length", item.Length, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Width", item.Width, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Height", item.Height, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SellingPrice", item.SellingPrice, DbType.String, ParameterDirection.Input);

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {

                const string storedProcedure = "dbo.Item_InsertItem";

                IDbTransaction transaction = connection.BeginTransaction();

                int rowsaffected = connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                //int rowsaffected = connection.Insert<ItemMaster>(item, transaction);

                transaction.Commit();

                return rowsaffected > 0;
            }
        }

        public bool AddFBARequest(FbaRequest item) //-----
        {

            var parameters = new DynamicParameters();

            parameters.Add("@UserID", item.UserID, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemMasterID", item.ItemMasterID, DbType.String, ParameterDirection.Input);
            parameters.Add("@SellerIndex", item.SellerIndex, DbType.String, ParameterDirection.Input);
            parameters.Add("@ListinItemNo", item.ListingItemNo, DbType.String, ParameterDirection.Input);
            parameters.Add("@SKU", item.SKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@FNSKU", item.FNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@FNSKUValue", item.FNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBARootID", item.FBARootID, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBARecedQty", item.FBARecedQty, DbType.String, ParameterDirection.Input);
            parameters.Add("@RequestQty", item.RequestQty, DbType.String, ParameterDirection.Input);
            parameters.Add("@Comments", item.Comments, DbType.String, ParameterDirection.Input);
            parameters.Add("@LableLink", item.LableLink, DbType.String, ParameterDirection.Input);
            parameters.Add("@LableStatus", item.LableStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("@PriorityStatus", item.PriorityStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("@UKSold7Days", item.UKSold7Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@EUSold7Days", item.EUSold7Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@USASold7Days", item.USASold7Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@AUSold7Days", item.AUSold7Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@CASold7Days", item.CASold7Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@UKSold7DaysFNSKU", item.UKSold7DaysFNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@EUSold7DaysFNSKU", item.EUSold7DaysFNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@USASold7DaysFNSKU", item.USASold7DaysFNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@CASold7DaysFNSKU", item.CASold7DaysFNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@AUSold7DaysFNSKU", item.AUSold7DaysFNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@UKSold30Days", item.UKSold30Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@EUSold30Days", item.EUSold30Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@USASold30Days", item.USASold30Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@AUSold30Days", item.AUSold30Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@CASold30Days", item.CASold30Days, DbType.String, ParameterDirection.Input);
            parameters.Add("@UKFbaStock", item.UKFbaStock, DbType.String, ParameterDirection.Input);
            parameters.Add("@EUFbaStock", item.EUFbaStock, DbType.String, ParameterDirection.Input);
            parameters.Add("@USAFbaStock", item.USAFbaStock, DbType.String, ParameterDirection.Input);
            parameters.Add("@CAFbaStock", item.CAFbaStock, DbType.String, ParameterDirection.Input);
            parameters.Add("@AUFbaStock", item.AUFbaStock, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBAStockFNSKUUK", item.FBAStockFNSKUUK, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBAStockFNSKUEU", item.FBAStockFNSKUEU, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBAStockFNSKUUS", item.FBAStockFNSKUUS, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBAStockFNSKUCA", item.FBAStockFNSKUCA, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBAStockFNSKUAU", item.FBAStockFNSKUAU, DbType.String, ParameterDirection.Input);
            parameters.Add("@UKWarehouse", item.UKWarehouse, DbType.String, ParameterDirection.Input);
            parameters.Add("@EUWarehouse", item.EUWarehouse, DbType.String, ParameterDirection.Input);
            parameters.Add("@USAWarehouse", item.USAWarehouse, DbType.String, ParameterDirection.Input);
            parameters.Add("@AUWarehouse", item.AUWarehouse, DbType.String, ParameterDirection.Input);
            parameters.Add("@CAWarehouse", item.CAWarehouse, DbType.String, ParameterDirection.Input);



            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {

                const string storedProcedure = "dbo.FBA_InsertFBA";

                IDbTransaction transaction = connection.BeginTransaction();

                int rowsaffected = connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                //int rowsaffected = connection.Insert<ItemMaster>(item, transaction);

                transaction.Commit();

                return rowsaffected > 0;
            }
        }

        public ItemMaster GetsItemsByID(int itemID, int sellerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ItemMasterID", itemID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SellerID", sellerId, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.GetItemsInfoByID";

                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                return items.FirstOrDefault();
            }

            //           var Items = new List<ItemMaster>();
            //           using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            //           {
            //               string query = @"Select I.ItemName,I.ItemMasterID,M.ManufacturerName,(cast(Length as varchar(10)) + 'x' + cast(Width as varchar(10)) + 'x' + cast(Height as varchar(10))) As Dimension1, I.CartonQty,I.MasterCartonQty
            //                                       INTO #TempTable from  ItemMasters I inner join ItemCategories C on I.ItemCategoryID = c.ItemCategoryID inner join
            //                                       ItemManufacturers M on I.ItemManufacturerID = M.ItemManufacturerID WHERE I.ItemMasterID = @itemID
            //                               SELECT *,  

            //                            (Select SUM([OrderItems].[Quantity]) AS Quantity FROM[arsukeuro_mssql].[dbo].[ItemStock]
            //                                JOIN[arsukeuro_mssql].[dbo].[ItemMasters] ON[ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
            //                                JOIN[arsukeuro_mssql].[dbo].[Orders] ON[Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
            //                                JOIN[arsukeuro_mssql].[dbo].[OrderItems] ON[Orders].OrderID = [OrderItems].OrderID
            //                                WHERE OrderDate>= DATEADD(DAY, -7, GETDATE()) AND Orders.Country = 'GB' AND ItemStock.ItemMasterID = @itemID
            //                                GROUP BY[ItemStock].[ItemMasterID] ) AS UKSold7Days,

            //                            (SELECT SUM([OrderItems].[Quantity]) AS Quantity
            //                                FROM[arsukeuro_mssql].[dbo].[ItemStock]
            //                                JOIN[arsukeuro_mssql].[dbo].[ItemMasters] ON[ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
            //                                JOIN[arsukeuro_mssql].[dbo].[Orders] ON[Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
            //                                JOIN[arsukeuro_mssql].[dbo].[OrderItems] ON[Orders].OrderID = [OrderItems].OrderID
            //                                WHERE OrderDate>= DATEADD(DAY, -7, GETDATE()) AND Orders.Country = 'AU' AND ItemStock.ItemMasterID = @itemID
            //                                GROUP BY[ItemStock].[ItemMasterID] ) AS AUSold7Days,

            //                            (SELECT SUM([OrderItems].[Quantity]) AS Quantity

            //       FROM[arsukeuro_mssql].[dbo].[ItemStock]

            //       JOIN[arsukeuro_mssql].[dbo].[ItemMasters] ON[ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]

            //       JOIN[arsukeuro_mssql].[dbo].[Orders] ON[Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]

            //       JOIN[arsukeuro_mssql].[dbo].[OrderItems] ON[Orders].OrderID = [OrderItems].OrderID

            //       WHERE OrderDate>= DATEADD(DAY, -7, GETDATE()) AND Orders.Country = 'USA' AND ItemStock.ItemMasterID = @itemID

            //       GROUP BY[ItemStock].[ItemMasterID] ) AS USASold7Days,

            //	(Select

            //      SUM([OrderItems].[Quantity]) AS Quantity


            //       FROM[arsukeuro_mssql].[dbo].[ItemStock]

            //       JOIN[arsukeuro_mssql].[dbo].[ItemMasters] ON[ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]

            //       JOIN[arsukeuro_mssql].[dbo].[Orders] ON[Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]

            //       JOIN[arsukeuro_mssql].[dbo].[OrderItems] ON[Orders].OrderID = [OrderItems].OrderID

            //       WHERE OrderDate>= DATEADD(DAY, -30, GETDATE()) AND Orders.Country = 'GB' AND ItemStock.ItemMasterID = @itemID

            //       GROUP BY[ItemStock].[ItemMasterID] ) AS UKSold30Days,

            //(SELECT SUM([OrderItems].[Quantity]) AS Quantity


            //       FROM[arsukeuro_mssql].[dbo].[ItemStock]

            //       JOIN[arsukeuro_mssql].[dbo].[ItemMasters] ON[ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]

            //       JOIN[arsukeuro_mssql].[dbo].[Orders] ON[Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]

            //       JOIN[arsukeuro_mssql].[dbo].[OrderItems] ON[Orders].OrderID = [OrderItems].OrderID

            //       WHERE OrderDate>= DATEADD(DAY, -30, GETDATE()) AND Orders.Country = 'AU' AND ItemStock.ItemMasterID = @itemID

            //       GROUP BY[ItemStock].[ItemMasterID] ) AS AUSold30Days,

            //                    (SELECT SUM([OrderItems].[Quantity]) AS Quantity FROM[arsukeuro_mssql].[dbo].[ItemStock]
            //                       JOIN[arsukeuro_mssql].[dbo].[ItemMasters] ON[ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
            //                       JOIN[arsukeuro_mssql].[dbo].[Orders] ON[Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
            //                       JOIN[arsukeuro_mssql].[dbo].[OrderItems] ON[Orders].OrderID = [OrderItems].OrderID
            //                       WHERE OrderDate>= DATEADD(DAY, -30, GETDATE()) AND Orders.Country = 'USA' AND ItemStock.ItemMasterID = @itemID
            //                       GROUP BY[ItemStock].[ItemMasterID] ) AS USASold30Days,

            //   (SELECT StockView.CurrentStock FROM  StockView WHERE StockView.ItemMasterID = @ItemMasterID) AS UKWarehouse



            //     FROM #TempTable WHERE TempTable.ItemMasterID = @item";

            //               Items = connection.Query<ItemMaster>(query).ToList();
            //           }

            //           return Items;


        }

        public ItemMaster GetFBAStockByFNSKU(string SKU, int SellerIndex)
        {
            var FBAStockFNSKUData = new ItemMaster();

            string query = @"select sum(FBAStockFNSKUUK) as FBAStockFNSKUUK,
                                   sum(FBAStockFNSKUEU) as FBAStockFNSKUEU,
	                               sum(FBAStockFNSKUUS) as FBAStockFNSKUUS,
	                               sum(FBAStockFNSKUCA) as FBAStockFNSKUCA,
	                               sum(FBAStockFNSKUAU)as FBAStockFNSKUAU,
	                               (Select ISNULL(SUM([OrderItems].[Quantity]),0) AS Quantity
		                                FROM  [arsukeuro_mssql].[dbo].[ItemStock] 
		                                JOIN  [arsukeuro_mssql].[dbo].[ItemMasters] ON [ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
		                                JOIN  [arsukeuro_mssql].[dbo].[Orders] ON [Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
		                                JOIN  [arsukeuro_mssql].[dbo].[OrderItems] ON [Orders].OrderID = [OrderItems].OrderID 
		                                WHERE OrderDate>=DATEADD(DAY,-7,GETDATE()) AND Orders.MarketPlace = 'amazon.co.uk' AND ItemStock.Sku=@SKU
		                                GROUP BY [ItemStock].[ItemMasterID]) AS UKSold7DaysFNSKU,

	                                (SELECT ISNULL(SUM([OrderItems].[Quantity]),0) AS Quantity	
		                                FROM  [arsukeuro_mssql].[dbo].[ItemStock] 
		                                JOIN  [arsukeuro_mssql].[dbo].[ItemMasters] ON [ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
		                                JOIN  [arsukeuro_mssql].[dbo].[Orders] ON [Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
		                                JOIN  [arsukeuro_mssql].[dbo].[OrderItems] ON [Orders].OrderID = [OrderItems].OrderID 
		                                WHERE OrderDate>=DATEADD(DAY,-7,GETDATE()) AND Orders.MarketPlace = 'amazon.com.au' AND ItemStock.Sku=@SKU
		                                GROUP BY [ItemStock].[ItemMasterID] ) AS AUSold7DaysFNSKU,

	                                (SELECT ISNULL(SUM([OrderItems].[Quantity]),0) AS Quantity	
		                                FROM  [arsukeuro_mssql].[dbo].[ItemStock] 
		                                JOIN  [arsukeuro_mssql].[dbo].[ItemMasters] ON [ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
		                                JOIN  [arsukeuro_mssql].[dbo].[Orders] ON [Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
		                                JOIN  [arsukeuro_mssql].[dbo].[OrderItems] ON [Orders].OrderID = [OrderItems].OrderID 
		                                WHERE OrderDate>=DATEADD(DAY,-7,GETDATE()) AND Orders.MarketPlace = 'amazon.com' AND ItemStock.Sku=@SKU
		                                GROUP BY [ItemStock].[ItemMasterID] ) AS USASold7DaysFNSKU,

	                                (SELECT ISNULL(SUM([OrderItems].[Quantity]),0) AS Quantity
		                                FROM  [arsukeuro_mssql].[dbo].[ItemStock] 
		                                JOIN  [arsukeuro_mssql].[dbo].[ItemMasters] ON [ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
		                                JOIN  [arsukeuro_mssql].[dbo].[Orders] ON [Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
		                                JOIN  [arsukeuro_mssql].[dbo].[OrderItems] ON [Orders].OrderID = [OrderItems].OrderID 
		                                WHERE OrderDate>=DATEADD(DAY,-7,GETDATE()) AND Orders.MarketPlace = 'amazon.ca' AND ItemStock.Sku=@SKU
		                                GROUP BY [ItemStock].[ItemMasterID] ) AS CASold7DaysFNSKU,

	                                (SELECT ISNULL(SUM([OrderItems].[Quantity]),0) AS Quantity
		                                FROM  [arsukeuro_mssql].[dbo].[ItemStock] 
		                                JOIN  [arsukeuro_mssql].[dbo].[ItemMasters] ON [ItemStock].[ItemMasterID] = [ItemMasters].[ItemMasterID]
		                                JOIN  [arsukeuro_mssql].[dbo].[Orders] ON [Orders].[OrderReferenceNo] = [ItemStock].[OrderReferenceNo]
		                                JOIN  [arsukeuro_mssql].[dbo].[OrderItems] ON [Orders].OrderID = [OrderItems].OrderID 
		                                WHERE OrderDate>=DATEADD(DAY,-7,GETDATE()) AND Orders.MarketPlace IN ('amazon.it','amazon.fr','amazon.es','amazon.pl','amazon.nl','amazon.se','amazon.de','amazon.com.tr') AND ItemStock.Sku=@SKU
		                                GROUP BY [ItemStock].[ItemMasterID] ) AS EUSold7DaysFNSKU
	                            from 
	                            (select 
		                              Case When (FDBSF.Location='UK') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUUK,
		                              Case When (FDBSF.Location='EU') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUEU,
		                              Case When (FDBSF.Location='US') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUUS,
		                              Case When (FDBSF.Location='CA') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUCA,
		                              Case When (FDBSF.Location='AU') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUAU
		                              from FDBStockFile as FDBSF
		                              inner join SellerAccounts as SA on FDBSF.Seller=
				                        CASE
					                        WHEN SA.SellerID='Jumbo Deals' THEN 'NEEZ'
					                        ELSE SA.SellerID
				                        END
		                              where FDBSF.Sku=@SKU AND SA.SellerIndex=@SellerIndex 
	                              ) as Result";

            //string query = @"select 
            //               Case When (FDBSF.Location='UK') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUUK,
            //               Case When (FDBSF.Location='EU') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUEU,
            //               Case When (FDBSF.Location='US') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUUS,
            //               Case When (FDBSF.Location='CA') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUCA,
            //               Case When (FDBSF.Location='AU') then FDBSF.AfnTotalQuantity Else 0 End as FBAStockFNSKUAU
            //               from FDBStockFile as FDBSF
            //               inner join SellerAccounts as SA on FDBSF.Seller=SA.SellerID
            //               where FDBSF.Fnsku=@Fnsku AND SA.SellerIndex=@SellerIndex";

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                FBAStockFNSKUData = connection.Query<ItemMaster>(query, new { SKU = SKU, SellerIndex = SellerIndex }).FirstOrDefault();
            }

            ItemMaster itemMaster = new ItemMaster();

            if (FBAStockFNSKUData == null)
            {
                itemMaster.FBAStockFNSKUUK = 0;
                itemMaster.FBAStockFNSKUEU = 0;
                itemMaster.FBAStockFNSKUUS = 0;
                itemMaster.FBAStockFNSKUCA = 0;
                itemMaster.FBAStockFNSKUAU = 0;
                itemMaster.UKSold7DaysFNSKU = 0;
                itemMaster.AUSold7DaysFNSKU = 0;
                itemMaster.USASold7DaysFNSKU = 0;
                itemMaster.CASold7DaysFNSKU = 0;
                itemMaster.EUSold7DaysFNSKU = 0;
                return itemMaster;
            }
            else
            {
                if (FBAStockFNSKUData.FBAStockFNSKUUK == null) FBAStockFNSKUData.FBAStockFNSKUUK = 0;
                if (FBAStockFNSKUData.FBAStockFNSKUEU == null) FBAStockFNSKUData.FBAStockFNSKUEU = 0;
                if (FBAStockFNSKUData.FBAStockFNSKUUS == null) FBAStockFNSKUData.FBAStockFNSKUUS = 0;
                if (FBAStockFNSKUData.FBAStockFNSKUCA == null) FBAStockFNSKUData.FBAStockFNSKUCA = 0;
                if (FBAStockFNSKUData.FBAStockFNSKUAU == null) FBAStockFNSKUData.FBAStockFNSKUAU = 0;
                if (FBAStockFNSKUData.UKSold7DaysFNSKU == null) FBAStockFNSKUData.UKSold7DaysFNSKU = 0;
                if (FBAStockFNSKUData.AUSold7DaysFNSKU == null) FBAStockFNSKUData.AUSold7DaysFNSKU = 0;
                if (FBAStockFNSKUData.USASold7DaysFNSKU == null) FBAStockFNSKUData.USASold7DaysFNSKU = 0;
                if (FBAStockFNSKUData.CASold7DaysFNSKU == null) FBAStockFNSKUData.CASold7DaysFNSKU = 0;
                if (FBAStockFNSKUData.EUSold7DaysFNSKU == null) FBAStockFNSKUData.EUSold7DaysFNSKU = 0;
            }
            return FBAStockFNSKUData;
        }

        public bool UpdateFbaRequest(FbaRequest item, int userId)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<FbaRequest>(item);
            }

            return updateCompleted;
        }
        public bool DeleteFBARequest(int FBARequestID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                string query = string.Format("Update FBARequest Set Del='Y' where FBARequestID={0}", FBARequestID);
                int rowsAffected = connection.Execute(query, commandType: CommandType.Text);
                return rowsAffected > 0;
            }
            
        }

        public bool UpdateItemStock(OrderRequest items)
        {
            int rowsaffected = 0;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                IDbTransaction transaction = connection.BeginTransaction();

                foreach (var item in items.ItemsRequested)
                {
                    string updateQuery = string.Format("Update ItemMasters Set StockUnits={0} Where ItemMasterID={1}", item.RequestedItemQuantity, item.ItemMasterID);
                    rowsaffected += connection.Execute(updateQuery, commandType: CommandType.Text, transaction: transaction);

                }

                transaction.Commit();

                return rowsaffected == items.ItemsRequested.Count;
            }
        }

        public ItemMaster GetItemsByID(int itemID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemMaster item = connection.Get<ItemMaster>(itemID);

                return item;
            }

        }
        //--------------------------------------Report stored procedure 30-11-2020---------------------------------------//

        public List<SKUReport> GetSKUReport(string fromDate, string toDate,string Asin, string SKU, string ItemMasterID, string SellerID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<SKUReport>("SKU_Report", new { fromDate = fromDate, toDate = toDate, Asin = Asin, SKU=SKU, ItemMasterID = ItemMasterID, SellerID= SellerID }, commandType: CommandType.StoredProcedure);
                //foreach (var item in result)
                //{
                //    item.FBAHandling = GetFbaHandling(item);
                //    item.PromotionalCost = GetPromotionalCost(item);
                //    item.Errors = GetErrorCost(item); 
                //    item.Profit = GetProfit(item);
                //    item.ProfitMargin = GetProfitMargin(item);

                //}
                return result.ToList();
            }
        }

        public List<SKUReportForSmayaSalesAndSalsabilEuro> GetSKUReportForSmayaSalesAndSalsabilEuro(string fromDate, string toDate, string Asin, string SKU, string ItemMasterID, string SellerID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<SKUReportForSmayaSalesAndSalsabilEuro>("SKU_Report_ForSmayaSalesAndSalsabilEuro", new { fromDate = fromDate, toDate = toDate, Asin = Asin, SKU = SKU, ItemMasterID = ItemMasterID, SellerID = SellerID }, commandType: CommandType.StoredProcedure);
                
                return result.ToList();
            }
        }
        
        private string GetProfitMargin(SKUReport item)
        {
            var ProfitMargin = Math.Round(((Convert.ToDouble(item.Profit) / (Convert.ToDouble(item.ItemCost))) * 100), 2).ToString();
            return Math.Round(Convert.ToDouble(item.Profit), 2).ToString();
        }

        private string GetProfit(SKUReport item)
        {
            var Profit = Math.Round(Convert.ToDouble(item.AmountPaid) -
                    (Convert.ToDouble(item.ItemCost) + Convert.ToDouble(item.FbaFees) +
                    Convert.ToDouble(item.SellingFees) + Convert.ToDouble(item.VatTax) +
                    Convert.ToDouble(item.FBAHandling) + Convert.ToDouble(item.PromotionalCost) +
                    Convert.ToDouble(item.Errors)), 2).ToString();
            return Math.Round(Convert.ToDouble(Profit), 2).ToString();
        }

        private string GetErrorCost(SKUReport item)
        {
            decimal amount = item.AmountPaid;
            var percent = (5 * amount) / 100;
            return Math.Round(percent, 2).ToString();
        }

        private string GetPromotionalCost(SKUReport item)
        {
            decimal amount = item.AmountPaid;
            var percent = (10 * amount) / 100;
            return Math.Round(percent, 2).ToString();
        }

        private string GetFbaHandling(SKUReport item)
        {
            decimal amount = item.AmountPaid;
            var percent = (5 * amount) / 100;
            return Math.Round(percent, 2).ToString();
        }

        public List<Sku_Report> Get_SKUReport(string fromDate, string toDate, string Asin, string SKU, string SellerID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<Sku_Report>("Get_SkuReport", new { fromDate = fromDate, toDate = toDate, Asin = Asin, SKU = SKU, SellerID= int.Parse(SellerID.Trim()) }, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public List<Asin_Report> Get_AsinReport(string fromDate, string toDate, string Asin,string ItemMasterID) //Asin Report 
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<Asin_Report>("Get_AsinReport", new { fromDate = fromDate, toDate = toDate, Asin = Asin ,ItemMasterID = ItemMasterID}, commandType: CommandType.StoredProcedure,commandTimeout:0);

                return result.ToList();
            }
        }

        public List<AsinSub_ReportModel> Get_AsinSubReport(string fromDate, string toDate, string Asin) //Item  Data  
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
              
                 var result = connection.Query<AsinSub_ReportModel>("Get_AsinSubReport", new { fromDate = fromDate, toDate = toDate, Asin = Asin }, commandType: CommandType.StoredProcedure,commandTimeout:0);
                
                return result.ToList();
            }
        }

        public List<ItemSummy> Get_ReportItemSummary(string fromDate, string toDate, string Asin) //Item Summary Data 
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<ItemSummy>("Get_ItemSummaryReport", new { fromDate = fromDate, toDate = toDate, Asin = Asin }, commandType: CommandType.StoredProcedure,commandTimeout:0);

                return result.ToList();
            }
        }

        //------------------------------------------------------  ------------------------------------------------------//

        //-----------------------------------GROUP BY REPORT 02-12-2021 ----------------------------------//

        public List<GroupReportModel> Get_GroupByReport(string fromDate, string toDate, string UserId) //Item Summary Data 
        {

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<GroupReportModel>("Get_GroupByReport", new { fromDate = fromDate, toDate = toDate, UserId = UserId }, commandType: CommandType.StoredProcedure, commandTimeout:0);

                return result.ToList();
            }
        }

        public List<GroupReportSummaryModel> Get_GroupSummary(string fromDate, string toDate, string UserId) //Item Summary Data 
        {

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<GroupReportSummaryModel>("Get_GroupSummary", new { fromDate = fromDate, toDate = toDate, UserId = UserId }, commandType: CommandType.StoredProcedure,commandTimeout:0);

                return result.ToList();
            }
        }

        public List<GroupData> Get_SumGroupReport(string fromDate, string toDate, string UserId) //Item Summary Data 
        {

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<GroupData>("Get_GroupSum", new { fromDate = fromDate, toDate = toDate, UserId = UserId }, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }
        //------------------------------------------------------  ------------------------------------------------------//

        //______________________________________________________________GET SKU BY ITEM_______________________________________________________________

        public List<ItemMaster> GetAsinByID(int ItemMasterID = 0)
        {


            var parameters = new DynamicParameters();
            parameters.Add("@ItemMasterID", ItemMasterID, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.GetAsinByID";
                IEnumerable<ItemMaster> AsinList = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return AsinList.ToList();
            }

        }

        public List<ItemMaster> GetUserItemByID(int UserID = 0)
        {
            

            var parameters = new DynamicParameters();
            parameters.Add("@UserID", UserID, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.GetUserItemByID";
                IEnumerable <ItemMaster> AsinList = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return AsinList.ToList();
                }
            
        }

        public List<ItemMaster> GetFNSKUListById(string SKU ="") 
        {

            var parameters = new DynamicParameters();
            parameters.Add("@ItemMasterID", SKU, DbType.String, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.GetFNSKUByID";
                IEnumerable<ItemMaster> AsinList = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return AsinList.ToList();
            }
        }

        public List<ItemMaster> GetSKUByID(string ListingItemNo )
        {


            var parameters = new DynamicParameters();
            parameters.Add("@ListingItemNo", ListingItemNo, DbType.String, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.GetSKUByID";
                IEnumerable<ItemMaster> SKUList = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return SKUList.ToList();
            }

        }

        public List<ItemMaster> GetItemsListByID(int SellerID)
        {


            var parameters = new DynamicParameters();
            parameters.Add("@SellerID", SellerID, DbType.String, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.GetItemsListByID";
                IEnumerable<ItemMaster> ItemsList = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return ItemsList.ToList();
            }

        }
        //________________________________________________________18-02-2021________________________________________________________________//

        public List<FBALocations> InventoryLocation()
        {
                var parameters = new DynamicParameters();

                using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
                {
                    string query = string.Format("Select FBARootID,FBARoot  from FBARoot Where Del = 'N' ");
                    IEnumerable<FBALocations> items = connection.Query<FBALocations>(query, parameters, commandType: CommandType.Text);
                    return items.ToList();
                }

        }

        //____________________________________________________________________________________________________________________________________________
        public List<ReportSummary> GetSalesReport(string fromDate, string toDate)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var result = connection.Query<ReportSummary>("Sales_Report", new { fromDate = fromDate, toDate = toDate }, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public bool InsertHistory(ItemMaster item, int userId = 0)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemHistory history = new ItemHistory();

                CopyPropertiesTo(item, history);

                history.ModifiedByUser = userId;
                history.ModifiedDate = DateTime.Now;

                connection.Insert<ItemHistory>(history);

                return true;
            }
        }

        public static void CopyPropertiesTo(object fromObject, object toObject)
        {
            PropertyInfo[] toObjectProperties = toObject.GetType().GetProperties();
            foreach (PropertyInfo propTo in toObjectProperties)
            {
                PropertyInfo propFrom = fromObject.GetType().GetProperty(propTo.Name);
                if (propFrom != null && propFrom.CanWrite)
                    propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
            }
        }

        public bool UpdateItems(ItemMaster item, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ItemMasterID", item.ItemMasterID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemName", item.ItemName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemCode", item.ItemCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemDescription", item.Description, DbType.String, ParameterDirection.Input);
            parameters.Add("@Brand", item.Brand, DbType.String, ParameterDirection.Input);
            parameters.Add("@Dimension", item.Dimension, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemWeight", item.ItemWeight, DbType.String, ParameterDirection.Input);
            parameters.Add("@VAT", item.VAT, DbType.String, ParameterDirection.Input);
            parameters.Add("@ItemCost", item.ItemCost, DbType.String, ParameterDirection.Input);
            parameters.Add("@BarCode", item.BarCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@SupplierID", item.SupplierID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemCategoryID", item.ItemCategoryID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemManufacturerID", item.ItemManufacturerID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ItemColorID", item.ItemColorID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@LocationID", item.LocationID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@StockUnits", 0, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ReOrderLevel", item.ReOrderLevel, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@MasterCartonQTY", item.MasterCartonQty, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CartonQty", item.CartonQty, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@LeadTime", item.LeadTime, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FNSKU", item.FNSKU, DbType.String, ParameterDirection.Input);
            parameters.Add("@Notes", item.Notes, DbType.String, ParameterDirection.Input);
            parameters.Add("@DateOfSubmission", item.DateOfSubmission, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Length", item.Length, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Width", item.Width, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Height", item.Height, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SellingPrice", item.SellingPrice, DbType.String, ParameterDirection.Input);

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {

                const string storedProcedure = "dbo.Item_UpdateItem";

                IDbTransaction transaction = connection.BeginTransaction();

                int rowsaffected = connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
               
                transaction.Commit();
                
                if (rowsaffected > 0)
                {
                    InsertHistory(item, userId);
                    return true;
                }

                return false;
            }
        }

        public bool QuickUpdate(ItemMaster item)
        {
            int rows = 0;

            var history = new ItemMaster();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                rows = connection.Execute("Update ItemMasters SET StockUnits=@units WHERE ItemMasterID=@id", new { units = 0, id = item.ItemMasterID });

                history = connection.Get<ItemMaster>(item.ItemMasterID);
            }

            if (rows > 0)
                InsertHistory(history);

            return rows > 0;
        }




        public bool DeleteIem(int itemID)
        {

            bool deleteCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var predicate = Predicates.Field<ItemMaster>(i => i.ItemMasterID, Operator.Eq, true);
                deleteCompleted = connection.Delete<ItemMaster>(predicate);
            }

            return deleteCompleted;
        }

        public List<Supplier> GetSuppliers()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<Supplier> items = connection.GetList<Supplier>().ToList();

                return items;
            }
        }

        public List<ItemCategory> GetCategories()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<ItemCategory> items = connection.GetList<ItemCategory>().ToList();

                return items;
            }
        }

        public ItemCategory GetCategoryByID(int categoryID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemCategory item = connection.Get<ItemCategory>(categoryID);

                return item;
            }
        }

        public bool AddItemCategory(ItemCategory category)
        {
            int categoryID;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                categoryID = connection.Insert<ItemCategory>(category);

                return categoryID > 0;
            }
        }

        public bool UpdateItemCategory(ItemCategory category)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<ItemCategory>(category);

                return updateCompleted;
            }
        }


        public List<ItemManufacturer> GetManufacturers()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<ItemManufacturer> items = connection.GetList<ItemManufacturer>().ToList();

                return items;
            }
        }

        public ItemManufacturer GetItemManufacturerByID(int manufacturerID)
        {

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemManufacturer item = connection.Get<ItemManufacturer>(manufacturerID);

                return item;
            }

        }
        public bool AddItemManufacturer(ItemManufacturer manufacturer)
        {

            int manufacturerID;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                manufacturerID = connection.Insert<ItemManufacturer>(manufacturer);

                return manufacturerID > 0;
            }

        }
        public bool UpdateItemManufacturer(ItemManufacturer manufacturer)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<ItemManufacturer>(manufacturer);

                return updateCompleted;
            }

        }




        public List<ItemColor> GetColors()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<ItemColor> items = connection.GetList<ItemColor>().ToList();

                return items;
            }
        }


        public ItemColor GetItemColorByID(int colorID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemColor item = connection.Get<ItemColor>(colorID);

                return item;
            }
        }

        public bool AddItemColor(ItemColor color)
        {
            int colorID;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                colorID = connection.Insert<ItemColor>(color);

                return colorID > 0;
            }
        }

        public bool UpdateItemColor(ItemColor color)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<ItemColor>(color);

                return updateCompleted;
            }
        }



        public List<ItemLocation> GetLocations()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<ItemLocation> items = connection.GetList<ItemLocation>().ToList();

                return items;
            }
        }


        public ItemLocation GetItemLocationByID(int locationID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemLocation item = connection.Get<ItemLocation>(locationID);

                return item;
            }
        }

        public bool AddItemLocation(ItemLocation location)
        {
            int locationID;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                locationID = connection.Insert<ItemLocation>(location);

                return locationID > 0;
            }
        }

        public bool UpdateItemLocation(ItemLocation location)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<ItemLocation>(location);

                return updateCompleted;
            }
        }

        public ItemMaster GetItemByID(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ItemID", id, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.Item_GetItemByIDForItemMaster";
                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure,commandTimeout:0);
                return items.FirstOrDefault();
            }
        }
        

        public List<ItemMaster> GetItemsBySupplier(int supplierID = 0)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SupplierID", supplierID, DbType.Int32, ParameterDirection.Input);

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.Item_GetAllItemsBySupplier";
                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }

        public List<EbayCategory> GetEbayCategories()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<EbayCategory> items = connection.GetList<EbayCategory>().ToList();

                return items;
            }
        }

        public EbayCategory GetEbayCategoryByID(int categoryID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                EbayCategory item = connection.Get<EbayCategory>(categoryID);

                return item;
            }
        }

        public bool AddEbayItemCategory(EbayCategory category)
        {
            int categoryID;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                categoryID = connection.Insert<EbayCategory>(category);

                return categoryID > 0;
            }
        }

        public bool UpdateEbayItemCategory(EbayCategory category)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<EbayCategory>(category);

                return updateCompleted;
            }
        }



        public List<AmazonCategory> GetAmazonCategories()
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                List<AmazonCategory> items = connection.GetList<AmazonCategory>().ToList();

                return items;
            }
        }

        public AmazonCategory GetAmazonCategoryByID(int categoryID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                AmazonCategory item = connection.Get<AmazonCategory>(categoryID);

                return item;
            }
        }

        public bool AddAmazonItemCategory(AmazonCategory category)
        {
            int categoryID;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                categoryID = connection.Insert<AmazonCategory>(category);

                return categoryID > 0;
            }
        }

        public string GetCurrency(string marketPlace)
        { 
                if (marketPlace.ToLower().Trim() == "uk")
                {
                    return "GBP";
                }
                else if (marketPlace.ToLower().Trim() == "it")
                {
                    return "EURO";
                }
                else if (marketPlace.ToLower().Trim() == "fr")
                {
                    return "EURO";
                }
                else if (marketPlace.ToLower().Trim() == "de")
                {
                    return "EURO";
                }
                else if (marketPlace.ToLower().Trim() == "es")
                {
                    return "EURO";
                }
                else if (marketPlace.ToLower().Trim() == "nl")
                {
                    return "EURO";
                }
                else if (marketPlace.ToLower().Trim() == "se")
                {
                    return "SEK";
                }
                else if (marketPlace.ToLower().Trim() == "pl")
                {
                    return "PLN";
                }
                else if (marketPlace.ToLower().Trim() == "us")
                {
                    return "USD";
                }
                else if (marketPlace.ToLower().Trim() == "au")
                {
                    return "AU$";
                }
                else
                {
                    return "EURO";
                }
            
        }

        public bool UpdateAmazonItemCategory(AmazonCategory category)
        {
            bool updateCompleted = false;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                updateCompleted = connection.Update<AmazonCategory>(category);

                return updateCompleted;
            }
        }

        public ItemMaster GetItemInfoByID(int itemID)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ItemMasterID", itemID, DbType.Int32, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.Listing_GetItemInfoByID";
                IEnumerable<ItemMaster> items = connection.Query<ItemMaster>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return items.FirstOrDefault();
            }
        }

        

        public List<BulkInsert> CreateListings(int[] items)
        {
            string itemsXml = string.Empty;
            itemsXml += "<root>";

            foreach (var item in items)
            {
                itemsXml += string.Format("<Access ItemID=\"{0}\"/>", item);
            }

            itemsXml += "</root>";
            var parameters = new DynamicParameters();
            parameters.Add("@XmlItemID", itemsXml, DbType.Xml, ParameterDirection.Input);
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                const string storedProcedure = "dbo.Item_CreateListing";
                IEnumerable<BulkInsert> item = connection.Query<BulkInsert>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return item.ToList();
            }
        }
    }
}
