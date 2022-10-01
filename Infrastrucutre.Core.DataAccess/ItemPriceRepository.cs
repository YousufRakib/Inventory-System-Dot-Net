using Dapper;
using DapperExtensions;
using Infrastructure.ConfigurationProvider;
using Infrastructure.Core.DataAccess;
using Infrastrucutre.Core.DataAccess.Interfaces;
using Infrastrucutre.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.DataAccess
{

    public class ItemPriceRepository : IItemPriceRepository
    {
        public List<ItemPriceViewModel> GetItemPrices()
        {
            var parameters = new DynamicParameters();

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                //try
                //{
                //    const string storedProcedure1 = "dbo.ItemPrice_GetAllItemPrices";
                //    IEnumerable<ItemPrice> items1 = connection.Query<ItemPrice>(storedProcedure1, parameters, commandType: CommandType.StoredProcedure);
                //}
                //catch (Exception ex)
                //{

                //}
                const string storedProcedure = "dbo.ItemPrice_GetAllItemPrices";
                IEnumerable<ItemPriceViewModel> items = connection.Query<ItemPriceViewModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }
        }

        public List<ItemPriceViewModel> GetItemPricesHistory(int id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            string query = string.Format(@"SELECT  IPH.ItemPriceID, IPH.ItemMasterID,IPH.FBARootId, IPH.UpdatedDate, itemMaster.ItemName as ItemName,  fba.FBARoot as FBARootName,IPH.OriginalPrice, IPH.Vat,  Price FROM ItempricesHistory as IPH
                                                left join dbo.ItemMasters as itemMaster on  IPH.ItemMasterID = itemMaster.ItemMasterID
                                                left join dbo.FBARoot as fba on  IPH.FBARootId = fba.FBARootID 
                                                WHERE IPH.ItemPriceID = @id
                                                ORDER BY IPH.ItemPriceHistoryID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;", jtStartIndex, jtPageSize);

            string countQuery = @"Select COUNT(*) FBASellerName from ItempricesHistory as IPH
                                  Where IPH.ItemPriceID = @id;";
            rowCount = 0;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var reader = connection.QueryMultiple(countQuery + query, new { id = id });
                rowCount = reader.Read<int>().Single();
                var result = reader.Read<ItemPriceViewModel>();

                return result.ToList();
            }
        }

        public bool RemoveItemPrice(int id)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                //connection.Execute(string.Format("UPDATE ItemPrices SET IsItActive=1 WHERE ItemPriceID={0}", id)); 
                //string query = string.Format("DELETE ItemPrices WHERE ItemPriceID = '{0}'", id);
                string query = string.Format("UPDATE ItemPrices SET IsItActive=0 WHERE ItemPriceID={0}", id);
                int rowsAffected = connection.Execute(query, commandType: CommandType.Text);
                return rowsAffected > 0;
            }
        }

        public ItemPrice GetItemPriceByID(int ItemPriceID)
        {
            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemPrice item = connection.Get<ItemPrice>(ItemPriceID);

                return item;
            }
        }

        public bool UpdateItemPrice(ItemPrice s)
        {
            bool ItemPriceUpdated = false;
            s.UpdatedDate = DateTime.Now;

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                ItemPriceUpdated = connection.Update<ItemPrice>(s);

                if (ItemPriceUpdated == true)
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@ItemPriceID", s.ItemPriceID, DbType.String, ParameterDirection.Input);
                    parameters.Add("@ItemMasterID", s.ItemMasterID, DbType.String, ParameterDirection.Input);
                    parameters.Add("@FBARootId", s.FBARootId, DbType.String, ParameterDirection.Input);
                    parameters.Add("@OriginalPrice", s.OriginalPrice, DbType.Double, ParameterDirection.Input);
                    parameters.Add("@Vat", s.Vat, DbType.Double, ParameterDirection.Input);
                    parameters.Add("@Price", s.Price, DbType.Double, ParameterDirection.Input);
                    parameters.Add("@IsItActive", true, DbType.Double, ParameterDirection.Input);

                    const string storedProcedure = "dbo.ItemPrice_AddItemPriceHistory";
                    IDbTransaction transaction = connection.BeginTransaction();
                    int rowsaffected = connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    return rowsaffected > 0;
                } 

                return ItemPriceUpdated;
            }
        }


        public bool AddItemPrice(ItemPrice s)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ItemMasterID", s.ItemMasterID, DbType.String, ParameterDirection.Input);
            parameters.Add("@FBARootId", s.FBARootId, DbType.String, ParameterDirection.Input);
            parameters.Add("@OriginalPrice", s.OriginalPrice, DbType.Double, ParameterDirection.Input);
            parameters.Add("@Vat", s.Vat, DbType.Double, ParameterDirection.Input);
            parameters.Add("@Price", s.Price, DbType.Double, ParameterDirection.Input);
            parameters.Add("@IsItActive", true, DbType.Double, ParameterDirection.Input);

            using (IDbConnection connection = DataAccessHelper.OpenSqlConnection(ConnectionStringManager.SqlConnectionStringInstance))
            {
                var query = string.Format(@"Update ItemPrices set  IsItActive = 0
 where ItemMasterID = {0} And FBARootID = {1};", s.ItemMasterID, s.FBARootId);

                int count = connection.Execute(query);
                //int totalCount = reader.Read<int>().Single();
                //var result = reader.Read<ItemPrice>();





                //s.IsItActive = true;
                const string storedProcedure = "dbo.ItemPrice_AddItemPrice";
                IDbTransaction transaction = connection.BeginTransaction();
                int rowsaffected = connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                transaction.Commit();
                return rowsaffected > 0;
            }
        }


    }
}
