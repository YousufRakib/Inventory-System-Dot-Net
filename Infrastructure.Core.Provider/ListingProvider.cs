using Infrastrucutre.Core.DataAccess;
using Infrastrucutre.Core.DataAccess.Interfaces;
using Infrastrucutre.Core.Models;
using Infrastrucutre.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Provider
{
    public class ListingProvider : IListingProvider
    {

        IListingRepository _listingRepository;
        IItemRepository _itemRepository;
        IItemPriceRepository _itemPriceRepository;

        //-------------------------------------------Load Lists------------------------------------------------------//
        public List<ItemMaster> GetItemsList(string ItemName = "") //Get Item List 15-10-2020 # Danish
        {
            return _listingRepository.GetItemsList(ItemName);
        }

        public List<ItemMaster> GetSKUList(string SKU = "") //Get Item List 24-10-2020 # Danish
        {
            return _listingRepository.GetSKUList(SKU);
        }


        public List<ItemMaster> GetAsinsList(string SKU = "") //Get Asin No List 24-10-2020 # Danish
        {
            return _listingRepository.GetAsinsList(SKU);
        }
        public List<ItemMaster> GetUserItemByID(string UserID = "") //Get User Item List 24-10-2020 # Danish
        {
            return _listingRepository.GetUserItemByID(UserID);
        }

        public List<ItemMaster> GetAsinByID(string ItemMasterID = "") //Get Asin No List 24-10-2020 # Danish
        {
            return _listingRepository.GetAsinByID(ItemMasterID);
        }
        public List<ItemMaster> GetSKUByID(string ListingItemNo = "") //Get Asin No List 24-10-2020 # Danish
        {
            return _listingRepository.GetSKUByID(ListingItemNo);
        }

        public List<UserInformation> GetUser(string UserID = "", bool? sync = false) //Get Asin No List 24-10-2020 # Danish
        {
            return _listingRepository.GetUser(UserID, sync);
        }

        public List<ItemDetails> GetFNSKUList(string FNSKU = "") //Get FNSKU List 24-10-2020 # Danish
        {
            return _listingRepository.GetFNSKUList(FNSKU);
        }
        public List<FBALocations> GetFBARootList(string FBARootID = "", bool? sync = false) //Get FNSKU List 24-10-2020 # Danish
        {
            return _listingRepository.GetFBARootList(FBARootID, sync);
        }

        //-------------------------------------------28-10-2020---------------------------------------------------------//
        public ListingProvider(IListingRepository listingRepository)
        {
            this._listingRepository = listingRepository;
        }
        //-------------------------------------------28-10-2020---------------------------------------------------------//
        public ListingProvider(IListingRepository listingRepository, IItemRepository itemRepository, IItemPriceRepository itemCostRepository)
        {
            this._listingRepository = listingRepository;
            this._itemRepository = itemRepository;
            this._itemPriceRepository = itemCostRepository;
        }

        public SellerItemLink InsertLink(SellerItemLink link)
        {
            return _listingRepository.InsertLink(link);
        }

        public bool UpdateLink(SellerItemLink link)
        {
            return _listingRepository.UpdateLink(link);
        }

        public bool DeleteLink(int ItemLinkId) //Delete Sku 
        {
            return _listingRepository.DeleteLink(ItemLinkId);
        }

        public bool RefreshOrderItems(string orderReferenceNo, List<OrderItem> items)
        {
            return _listingRepository.RefreshOrderItems(orderReferenceNo, items);
        }

        public string UploadFDBStockFile(DataSet vmOrderSheet)
        {
            try
            {
                string firstColumn = vmOrderSheet.Tables[0].Rows[0][0].ToString();
                string secondColumn = vmOrderSheet.Tables[0].Rows[0][1].ToString();

                if (firstColumn == "seller" && secondColumn == "location")
                {
                    List<FDBStockFile> fDBStockFileList = new List<FDBStockFile>();
                    foreach (DataRow row in vmOrderSheet.Tables[0].Rows.Cast<DataRow>().Skip(1))
                    {
                        FDBStockFile fDBStockFile = new FDBStockFile();
                        fDBStockFile.Seller = row[0].ToString().ToUpper();
                        fDBStockFile.Location = row[1].ToString().ToUpper();
                        fDBStockFile.Sku = row[2].ToString();
                        fDBStockFile.Fnsku = row[3].ToString();
                        fDBStockFile.Asin = row[4].ToString();
                        fDBStockFile.ProductName = row[5].ToString();
                        fDBStockFile.Condition = row[6].ToString();
                        fDBStockFile.YourPrice = (row[7].ToString() == null || row[7].ToString() == "") ? Convert.ToDecimal(0) : Convert.ToDecimal(row[7].ToString());
                        fDBStockFile.MfnListingExists = row[8].ToString();
                        fDBStockFile.MfnFulfillableQuantity = (row[9].ToString() == null || row[9].ToString() == "") ? 0 : Convert.ToInt32(row[9].ToString());
                        fDBStockFile.AfnListingExists = row[10].ToString();
                        fDBStockFile.AfnWarehouseQuantity = (row[11].ToString() == null || row[11].ToString() == "") ? 0 : Convert.ToInt32(row[11].ToString());
                        fDBStockFile.AfnFulfillableQuantity = (row[12].ToString() == null || row[12].ToString() == "") ? 0 : Convert.ToInt32(row[12].ToString());
                        fDBStockFile.AfnUnsellableQuantity = (row[13].ToString() == null || row[13].ToString() == "") ? 0 : Convert.ToInt32(row[13].ToString());
                        fDBStockFile.AfnReservedQuantity = (row[14].ToString() == null || row[14].ToString() == "") ? 0 : Convert.ToInt32(row[14].ToString());
                        fDBStockFile.AfnTotalQuantity = (row[15].ToString() == null || row[15].ToString() == "") ? 0 : Convert.ToInt32(row[15].ToString());
                        fDBStockFile.PerUnitVolume = (row[16].ToString() == null || row[16].ToString() == "") ? Convert.ToDecimal(0) : Convert.ToDecimal(row[16].ToString());
                        fDBStockFile.AfnInboundWorkingQuantity = (row[17].ToString() == null || row[17].ToString() == "") ? 0 : Convert.ToInt32(row[17].ToString());
                        fDBStockFile.AfnInboundShippedQuantity = (row[18].ToString() == null || row[18].ToString() == "") ? 0 : Convert.ToInt32(row[18].ToString());
                        fDBStockFile.AfnInboundReceivingQuantity = (row[19].ToString() == null || row[19].ToString() == "") ? 0 : Convert.ToInt32(row[19].ToString());
                        fDBStockFile.AfnResearchingQuantity = (row[20].ToString() == null || row[20].ToString() == "") ? 0 : Convert.ToInt32(row[20].ToString());
                        fDBStockFile.AfnReservedFutureSupply = (row[21].ToString() == null || row[21].ToString() == "") ? 0 : Convert.ToInt32(row[21].ToString());
                        fDBStockFile.AfnFutureSupplyBuyable = (row[22].ToString() == null || row[22].ToString() == "") ? 0 : Convert.ToInt32(row[22].ToString());
                        fDBStockFileList.Add(fDBStockFile);
                    }

                    return _listingRepository.InsertFDBStockFileData(fDBStockFileList);
                }
                else
                {
                    return "3";
                }
            }
            catch (Exception ex)
            {
                return "2";
            }
        }

        public List<AutoCompleteItem> GetSellerList()
        {
            return _listingRepository.GetSellerList();
        }

        public List<AutoCompleteItem> GetPostageList() // Get Postage List For SKU New Recorde
        {
            return _listingRepository.GetPostageList();
        }

        public bool AddListing(ListingRequest request, out int listingRequestID)
        {
            return _listingRepository.AddListing(request, out listingRequestID);
        }

        public bool AddFBARequest(FbaRequest request, out int FRequestID) //FBA Request Form 04-03-2021
        {
            return _listingRepository.FBARequest(request, out FRequestID);
        }

        public List<ListingRequest> GetListingRequests(int requestID = 0)
        {
            return _listingRepository.GetListingRequests(requestID);

        }
        //_______________FBA Req Updating____________________//

        public List<FbaRequest> GetFbaRequest(out int totalCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _listingRepository.GetFbaRequest(out totalCount, jtStartIndex, jtPageSize);

        }
        public bool UpdateFbaRequest(FbaRequest request)
        {
            return _listingRepository.UpdateFbaRequest(request);
        }

        public List<FbaRequest> GetFbaPendingRequest(int jtStartIndex = 0, int jtPageSize = 0, int SellerIndex = 0, int FBARootID = 0)
        {
            return _listingRepository.GetFbaPendingRequest(jtStartIndex, jtPageSize, SellerIndex, FBARootID);

        }

        public List<FBAPendingRequestSummary> GetFbaPendingRequestSummary()
        {
            return _listingRepository.GetFbaPendingRequestSummary();

        }

        public bool UpdateFbaProcesingByID(int FBARequestID = 0, string status="", string reason="") //Accept FBA Processing Form
        {
            return _listingRepository.UpdateFbaProcesingByID(FBARequestID,status,reason);

        }
        public bool RejectFbaProcesingByID(int FBARequestID = 0) //Accept FBA Processing Form
        {
            return _listingRepository.RejectFbaProcesingByID(FBARequestID);

        }
        //________Get Fba Sorted List 15/03/2021________//


        public List<FbaRequest> GetFbaSortedList(int jtStartIndex = 0, int jtPageSize = 0, int SellerIndex = 0, int FBARootID = 0)
        {
            return _listingRepository.GetFbaSortedList(jtStartIndex, jtPageSize, SellerIndex, FBARootID);

        }

        public bool AddShipment(string ShipmentID, string ShipmentMethod, string Destination, DateTime ProceedTime, decimal[] FinalQty, decimal[] FBABoxQty, int[] OrderIDs, string[] FNSKUs, int[] ItemMasterIDs, int[] FBARootIDs)
        {
            return _listingRepository.AddShipment(ShipmentID, ShipmentMethod, Destination, ProceedTime, FinalQty, FBABoxQty, OrderIDs,FNSKUs,ItemMasterIDs,FBARootIDs);
        }

        public List<ShipmentDetails> GetShipmentDetails(out int totalCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _listingRepository.GetShipmentDetails(out totalCount, jtStartIndex, jtPageSize);

        }
        public List<FbaRequest> GetShipmentHistory(string id, out int totalCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _listingRepository.GetShipmentHistory(id, out totalCount, jtStartIndex, jtPageSize);

        }
        public List<FbaRequest> GetFbaRequestHistory(string id, out int totalCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _listingRepository.GetFbaRequestHistory(id, out totalCount, jtStartIndex, jtPageSize);

        }
        public List<FbaRequestViewModel> GetFbaRequestHistoryByFNSKU(string id, out int totalCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _listingRepository.GetFbaRequestHistoryByFNSKU(id, out totalCount, jtStartIndex, jtPageSize);

        }

        public bool DeleteShipment(string ShipmentID = "") //Accept FBA Processing Form
        {
            return _listingRepository.DeleteShipment(ShipmentID);

        }
        public List<ShipmentDetails> GetTotalFNSKU(string ShipmentID = "") ///Get Total Fnsku
        {
            return _listingRepository.GetTotalFNSKU(ShipmentID);
        }

        //_______________________________________//

        //--------------------------------------------------- GROUP BY REPORT------------------------//
        public List<UserInformation> GetGroupUser(string UserID = "", bool? sync = false) //Get Group List 02-12-2021 # Danish
        {
            return _listingRepository.GetGroupUser(UserID, sync);
        }

        //------------------------------------------------------------------------------------------------------------------//

        public bool UpdateListing(ListingRequest request)
        {
            return _listingRepository.UpdateListing(request);
        }

        public bool SubmitListing(ListingSubmission submission)
        {
            return _listingRepository.SubmitListing(submission);
        }


        public ListingRequest GetListingRequestByID(int requestID)
        {
            return _listingRepository.GetListingRequestByID(requestID);
        }

        public FbaRequest GetFbaRequestByID(int FbarequestID)
        {
            return _listingRepository.GetFbaRequestByID(FbarequestID);
        }


        public List<ListingRequestFilter> GetUnAllocatedListingRequestNo()
        {
            return _listingRepository.GetUnAllocatedListingRequestNo();
        }


        public List<ListingStatus> GetListingStatus()
        {
            return _listingRepository.GetListingStatus();
        }


        public List<ListingChannel> GetListingChannels()
        {
            return _listingRepository.GetListingChannels();
        }


        public ListingSubmission GetListingSubmissionByID(int submissionID)
        {
            return _listingRepository.GetListingSubmissionByID(submissionID);
        }


        public bool UpdateSubmission(ListingSubmission submission)
        {
            return _listingRepository.UpdateSubmission(submission);
        }


        public List<Submission> GetListingSubmissions(int itemMasterID = 0)
        {
            return _listingRepository.GetListingSubmissions(itemMasterID);
        }


        public bool InsertOrders(List<Order> orders)
        {
            return _listingRepository.InsertOrders(orders);
        }

        public bool SynchronizeSKU()
        {
            return _listingRepository.SynchronizeSKU();
        }

        public List<ListingDocument> GetListingRequestImages(int requestID)
        {
            return _listingRepository.GetListingRequestImages(requestID);
        }


        public bool Uploadfile(string fileName, string filePath, int requestID)
        {
            return _listingRepository.Uploadfile(fileName, filePath, requestID);
        }

        public List<SellerItemLink> GetSellerItemLink(out int rowCount, int id, string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _listingRepository.GetSellerItemLink(out rowCount, id, filter, filterText, jtStartIndex, jtPageSize);
        }


        public bool DeleteFile(int requestID, int fileID)
        {
            return _listingRepository.DeleteFile(requestID, fileID);
        }


        public List<Order> GetListingOrders(int[] orderIDs = null, string startDate = null, string endDate = null, string sellerId = null, string dispatchStatus = null, string orderNumber = null, bool? specialDelivery = null, string jtSorting = null)
        {
            return _listingRepository.GetListingOrders(orderIDs, startDate, endDate, sellerId, dispatchStatus, orderNumber: orderNumber, specialDelivery: specialDelivery, jtSorting: jtSorting);
        }


        public bool CreateListingItems(List<BulkInsert> ItemsInserted)
        {
            return _listingRepository.CreateListingItems(ItemsInserted);
        }


        public List<SellerAccount> GetSeller(string sellerID = "", bool? sync = false)
        {
            return _listingRepository.GetSeller(sellerID, sync);
        }


        public bool UpdateCarrier(int PostalCarrierID, DateTime ProceedTime, int[] OrderIDs)
        {
            return _listingRepository.UpdateCarrier(PostalCarrierID, ProceedTime, OrderIDs);
        }
        public bool updateReactiveDeletedOrders(DateTime ProceedTime, int[] OrderIDs)
        {
            return _listingRepository.updateReactiveDeletedOrders(ProceedTime, OrderIDs);
        }
        public bool UpdateCarrierRef(string carrierRef, DateTime ProceedTime, int[] OrderIDs)
        {
            return _listingRepository.UpdateCarrierRef(carrierRef, ProceedTime, OrderIDs);
        }

        public bool UpdateOrderCarriers(List<Order> orders)
        {
            return _listingRepository.UpdateOrderCarriers(orders);
        }


        public bool CheckExistingOrder(string orderRefNo)
        {
            return _listingRepository.CheckExistingOrder(orderRefNo);
        }

        public List<ItemImage> GetItemCodes(string sellerID)
        {
            return _listingRepository.GetItemCodes(sellerID);
        }

        public bool InsertImages(List<ItemImage> images)
        {
            return _listingRepository.InsertImages(images);
        }

        public bool UpdateOrders(List<OrderItem> orderItems)
        {
            return _listingRepository.UpdateOrders(orderItems);
        }
        //public bool UpdateOrdersFromExcelAmazon(List<OrderItem> orderItems) {
        //    return _listingRepository.UpdateOrdersFromExcelAmazon(orderItems);
        //}

        public bool InsertSalesCommission(SaleOrder sale, out string errorMessage)
        {
            return _listingRepository.InsertSalesCommission(sale, out errorMessage);
        }

        public SellerItemLink UpdateSellerItemLink(SellerItemLink link)
        {
            return _listingRepository.UpdateSellerItemLink(link);
        }

        public List<SellerItemLink> GetItemSellerLink(int id)
        {
            return _listingRepository.GetItemSellerLink(id);
        }
        public List<SellerItemLink> GetItemSellerLinkBySKU(List<string> lstSku)
        {
            return _listingRepository.GetItemSellerLinkBySKU(lstSku);
        }
        public List<SellerItemLink> GetItemSellerLinkList()
        {
            return _listingRepository.GetItemSellerLinkList();
        }

        public List<SellerItemLink> GetListingLinkSubmission(int id)
        {
            return _listingRepository.GetListingLinkSubmission(id);
        }

        public bool UpdateSellerAccount(SellerAccount account)
        {
            return _listingRepository.UpdateSellerAccount(account);
        }
        public SellerAccount InsertSellerAccount(SellerAccount account)
        {
            return _listingRepository.InsertSellerAccount(account);
        }

        public List<SellerAccount> GetSellerAccounts()
        {
            return _listingRepository.GetSellerAccounts();
        }

        public bool UpdateSellerAuth(SellerAccount account)
        {
            return _listingRepository.UpdateSellerAuth(account);
        }

        public List<SellerItemLink> GetStockErrors()
        {
            return _listingRepository.GetStockErrors();
        }

        public SellerItemLink InsertAndFix(SellerItemLink link)
        {
            return _listingRepository.InsertAndFix(link);
        }

        public bool DeleteOrder(int orderID)
        {
            return _listingRepository.DeleteOrder(orderID);
        }
        public bool UpdateAdditionalNotes(string addiotionalNotes, int[] OrderIDs)
        {
            return _listingRepository.UpdateAdditionalNotes(addiotionalNotes, OrderIDs);
        }

        public bool AddGroup(int GID, DateTime ProceedTime, int[] OrderIDs)
        {
            return _listingRepository.AddGroup(GID, ProceedTime, OrderIDs);
        }

        public bool UpdateOrderItemsByOrderSheetUpload(DataSet vmOrderSheet)
        {
            //var dataTable = vmOrderSheet.Tables["sheet1"];
            var dataTable = vmOrderSheet.Tables["table1"];
            dataTable.Rows.Remove(dataTable.Rows[0]);
            //string fileFor = dataTable.Rows[0].ItemArray[7].ToString();
            string fileFor = "";

            List<ItemPrice> lstItemPriceList = new List<ItemPrice>();

            List<VM_OrderSheet_Upload> lstOrderSheetConvertedData = new List<VM_OrderSheet_Upload>();
            List<ItemMaster> lstItemMaster = _itemRepository.GetItemMasterList();
            List<SellerItemLink> lstSellerItemLink = _listingRepository.GetItemSellerLinkList();
            List<PostalCarrier> lstostalCarriers = _itemRepository.GetPostageList();

            //List<ItemPrice> lstItemPrice = _itemPriceRepository.GetItemPrices();

            foreach (DataRow item in dataTable.Rows)
            {
                
                //var a = item[2].ToString();
                //var a = DateTime.Parse(item[0].ToString().Replace("UTC","-0")); ;
                if (item[2].ToString() == "Order" || item[2].ToString() == "Bestellung" || item[2].ToString() == "Zamówienie" || item[2].ToString() == "Ordine" || item[2].ToString() == "Commande" || item[2].ToString() == "Pedido" || item[2].ToString() == "Bestelling" || item[2].ToString() == "Sipari?" || item[2].ToString() == "Pedido")
                {
                    VM_OrderSheet_Upload vmOrderSheetConvertedData = new VM_OrderSheet_Upload();
                    if (item[7].ToString() == "amazon.co.uk" || item[7].ToString() == "amazon.de" || item[7].ToString() == "amazon.it" || item[7].ToString() == "amazon.fr" || item[7].ToString() == "amazon.ca" || item[7].ToString() == "amazon.es")
                    {
                        if (string.IsNullOrEmpty(fileFor))
                        { fileFor = item[7].ToString(); }
                        LoadInfoForUkDeItFr(ref vmOrderSheetConvertedData, item);
                    }
                    else if (item[7].ToString() == "amazon.com.au" || item[7].ToString() == "amazon.pl" || item[7].ToString() == "amazon.nl" || item[7].ToString() == "amazon.se")
                    {
                        if (string.IsNullOrEmpty(fileFor))
                        { fileFor = item[7].ToString(); }
                        LoadInfoForAu(ref vmOrderSheetConvertedData, item);
                    }
                    else if (item[7].ToString() == "amazon.com")
                    {
                        if (string.IsNullOrEmpty(fileFor))
                        { fileFor = item[7].ToString(); }
                        LoadInfoForUsa(ref vmOrderSheetConvertedData, item);
                    }
                    else if (item[7].ToString() == "amazon.com.tr")
                    {
                        if (string.IsNullOrEmpty(fileFor))
                        { fileFor = item[7].ToString(); }
                        LoadInfoForTurkey(ref vmOrderSheetConvertedData, item);
                    }

                    lstOrderSheetConvertedData.Add(vmOrderSheetConvertedData);
                }
            }

            List<VMOrder> lstVmOrder = new List<VMOrder>();
            List<VMOrderItems> lstVmOrderItems = new List<VMOrderItems>();
            List<Order> lstOrderAdd = new List<Order>();


            List<string> referenceSkuList = lstOrderSheetConvertedData.Select(x => x.SKU).Distinct().ToList();

            List<string> referenceList = lstOrderSheetConvertedData.Select(x => x.OrderReferenceNo).Distinct().ToList();
            List<Order> lstOrderFromDb = _listingRepository.GetOrdersListByOrderReferenceId(referenceList);

            List<string> foundOrderRefNoList = lstOrderFromDb.Select(x => x.OrderReferenceNo).ToList();
            List<string> lstNotExist = referenceList.Where(x => !foundOrderRefNoList.Contains(x)).ToList();

            //referenceList = foundOrderRefNoList/*lstOrderFromDb.Select(x => x.OrderReferenceNo).ToList()*/;

            List<int> lstItemMasterList = GetItemMasterIdListBySKUId(referenceSkuList, lstSellerItemLink);//_listingRepository.GetItemMasterIdListBySKUId(referenceSkuList);
            lstItemPriceList = _listingRepository.GetItemPriceByItemMasterId(fileFor, lstItemMasterList);

            lstOrderSheetConvertedData = lstOrderSheetConvertedData.Where(x => referenceList.Contains(x.OrderReferenceNo)).ToList();
            var groupByOrderRefferenceId = lstOrderSheetConvertedData.GroupBy(x => x.OrderReferenceNo).ToList();

            List<int> lstOrderId = lstOrderFromDb.Select(x => x.OrderID).ToList();
            List<OrderItem> lstOrderItems = _listingRepository.GetOrderItemsListByOrderListId(lstOrderId);
            int setial = -1;
            foreach (var orderInGroupBy in groupByOrderRefferenceId)
            {
                setial++;
                List<OrderItem> lstOrderItemsAdd = new List<OrderItem>();
                var orderReferenceNo = orderInGroupBy.FirstOrDefault().OrderReferenceNo;
                var marketPlace = orderInGroupBy.FirstOrDefault().MarketPlace;
                var fulfilment = orderInGroupBy.FirstOrDefault().Fulfilment;
                var taxCollectionModel = orderInGroupBy.FirstOrDefault().TaxCollectionModel;
                if (orderReferenceNo == "203-3709940-9427529" /*|| orderReferenceNo == "026-1235341-4439552"*/)
                {

                }

                var orderInDb = lstOrderFromDb.Where(x => x.OrderReferenceNo == orderReferenceNo).FirstOrDefault();
                if (orderInDb != null)//meaning need to update
                {
                    //lstVmOrder.Add(new VMOrder() { OrderReferenceNo = orderReferenceNo, Fulfilment = fulfilment, MarketPlace = marketPlace, TaxCollectionModel = taxCollectionModel, Currency = GetCurrencyFrom(orderInDb.MarketPlace) });
                    
                    //here
                    //////lstVmOrder.Add(new VMOrder() { OrderReferenceNo = orderReferenceNo, Fulfilment = fulfilment, MarketPlace = marketPlace, TaxCollectionModel = taxCollectionModel, Currency = GetCurrencyFrom(marketPlace), Country = GetCountryName(marketPlace) });

                    var lstOfOrderItems = lstOrderItems.Where(x => x.OrderID == orderInDb.OrderID);
                    //var tempList = lstOfOrderItems.ToList();
                    foreach (var orderItem in lstOfOrderItems)
                    {
                        VMOrderItems vmOrderItems = new VMOrderItems();
                        //var templist1 = orderInGroupBy.ToList();

                        var SellerIndex = 3;
                        var SellerId = "arsuk";
                        var clientItem = orderInGroupBy.Where(x => x.SKU == orderItem.SKUItemID).FirstOrDefault();
                        if (clientItem != null)
                        {

                            vmOrderItems.OrderId = orderItem.OrderID;
                            vmOrderItems.ProductSalesPrice = clientItem.ProductSalesPrice;
                            vmOrderItems.ProductSalesTax = clientItem.ProductSalesTax;
                            vmOrderItems.PostageCredits = clientItem.PostageCredits;
                            vmOrderItems.ShippingCreditsTax = clientItem.ShippingCreditsTax;
                            vmOrderItems.GiftWrapCredits = clientItem.GiftWrapCredits;
                            vmOrderItems.GiftWrapCreditsTax = clientItem.GiftWrapCreditsTax;
                            vmOrderItems.PromotionalRebates = Math.Abs(clientItem.PromotionalRebates);
                            vmOrderItems.PromotionalRebatesTax = Math.Abs(clientItem.PromotionalRebatesTax);
                            vmOrderItems.MarketplaceWithHeldTax = clientItem.MarketplaceWithHeldTax;
                            vmOrderItems.SellingFees = Math.Abs(clientItem.SellingFees);
                            vmOrderItems.FbaFees = Math.Abs(clientItem.FbaFees);
                            vmOrderItems.OtherTransactionFees = clientItem.OtherTransactionFees;
                            vmOrderItems.Other = clientItem.Other;
                            //if (orderItem.ItemCost == 0)
                            try
                            {
                                vmOrderItems.ItemCost = GetCostForThisSKUWithSellerId(lstItemMaster, lstSellerItemLink, clientItem.SKU, lstItemPriceList, ref SellerId, ref SellerIndex);
                                                        //GetCostForThisSKU(lstItemMaster, lstSellerItemLink, clientItem.SKU, lstItemPriceList);
                                vmOrderItems.ActualDelivery = GetActualDeliveryThisSKU(lstItemMaster, lstSellerItemLink, lstostalCarriers, clientItem, marketPlace);
                            }
                            catch (Exception ex)
                            {

                            }
                            lstVmOrderItems.Add(vmOrderItems);
                        }
                        lstVmOrder.Add(new VMOrder() { OrderReferenceNo = orderReferenceNo, Fulfilment = fulfilment, MarketPlace = marketPlace, TaxCollectionModel = taxCollectionModel, 
                            Currency = GetCurrencyFrom(marketPlace), Country = GetCountryName(marketPlace),SellerID = SellerId,SellerIndex = SellerIndex  });

                    }
                }
                else
                {
                    if (orderReferenceNo != null)
                    {
                        var AmountPaid = orderInGroupBy.FirstOrDefault().ProductSalesPrice;
                        var Name = "";
                        var Street1 = "";
                        var Street2 = "";
                        var CityName = orderInGroupBy.FirstOrDefault().OrderCity;
                        var StateOrProvince = orderInGroupBy.FirstOrDefault().OrderState;
                        var Country = GetCountryName(marketPlace);
                        var PostalCode = orderInGroupBy.FirstOrDefault().OrderPostal;
                        //var OrderReferenceNo = orderInGroupBy.FirstOrDefault().OrderReferenceNo;
                        var OrderDate = orderInGroupBy.FirstOrDefault().DateTime;
                        var OrderStatus = "Unshipped";
                        var SellerId = "arsuk";
                        var CheckoutState = "Unshipped";
                        var ShippedDate = orderInGroupBy.FirstOrDefault().DateTime;
                        var SalesOrderNumber = orderInGroupBy.FirstOrDefault().OrderReferenceNo;
                        var Channel = orderInGroupBy.FirstOrDefault().Fulfilment;
                        var SellerIndex = 3;
                        var IsActive = true;


                        OrderItem vmOrderItems = new OrderItem();
                        foreach (var item in orderInGroupBy)
                        {
                            var clientItem = item;
                            if (clientItem != null)
                            {
                                //var itemId = lstSellerItemLink.Where(x => x.SKU == clientItem.SKU).FirstOrDefault() == null ? GetNewSellerItemLink(ref lstSellerItemLink, clientItem.SKU) : lstSellerItemLink.Where(x => x.SKU == clientItem.SKU).FirstOrDefault().ListingItemNo;
                                var itemId = lstSellerItemLink.Where(x => x.SKU == clientItem.SKU).FirstOrDefault() == null ? "" : lstSellerItemLink.Where(x => x.SKU == clientItem.SKU).FirstOrDefault().ListingItemNo;
                                vmOrderItems.ItemID = itemId;// lstSellerItemLink.Where(x => x.SKU == clientItem.SKU).FirstOrDefault().ListingItemNo;
                                vmOrderItems.ItemTitle = clientItem.Description;
                                vmOrderItems.Quantity = clientItem.Quantity;
                                vmOrderItems.ShippingCost = clientItem.ShippingCreditsTax;
                                //vmOrderItems.TransactionPrice = clientItem.OtherTransactionFees;
                                vmOrderItems.TransactionPrice = clientItem.total;
                                vmOrderItems.SKUItemID = clientItem.SKU;

                                vmOrderItems.ProductSalesPrice = clientItem.ProductSalesPrice;
                                vmOrderItems.ProductSalesTax = clientItem.ProductSalesTax;
                                vmOrderItems.PostageCredits = clientItem.PostageCredits;
                                vmOrderItems.ShippingCreditsTax = clientItem.ShippingCreditsTax;
                                vmOrderItems.GiftWrapCredits = clientItem.GiftWrapCredits;
                                vmOrderItems.GiftWrapCreditsTax = clientItem.GiftWrapCreditsTax;
                                vmOrderItems.PromotionalRebates = Math.Abs(clientItem.PromotionalRebates);
                                vmOrderItems.PromotionalRebatesTax = Math.Abs(clientItem.PromotionalRebatesTax);
                                vmOrderItems.MarketplaceWithHeldTax = clientItem.MarketplaceWithHeldTax;
                                vmOrderItems.SellingFees = Math.Abs(clientItem.SellingFees);
                                vmOrderItems.FbaFees = Math.Abs(clientItem.FbaFees);
                                vmOrderItems.OtherTransactionFees = clientItem.OtherTransactionFees;
                                vmOrderItems.Other = clientItem.Other;
                                vmOrderItems.TransactionPrice = clientItem.total;
                                //if (orderItem.ItemCost == 0)
                                try
                                {
                                    vmOrderItems.ItemCost = GetCostForThisSKUWithSellerId(lstItemMaster, lstSellerItemLink, clientItem.SKU, lstItemPriceList, ref SellerId,ref SellerIndex);
                                    vmOrderItems.ActualDelivery = GetActualDeliveryThisSKU(lstItemMaster, lstSellerItemLink, lstostalCarriers, clientItem, marketPlace);
                                }
                                catch (Exception ex)
                                {

                                }
                                lstOrderItemsAdd.Add(vmOrderItems);
                            }
                        }
                        lstOrderAdd.Add(new Order()
                        {
                            OrderReferenceNo = orderReferenceNo,
                            MarketPlace = marketPlace,
                            Fulfilment = fulfilment,
                            TaxCollectionModel = taxCollectionModel,
                            Currency = GetCurrencyFrom(marketPlace),
                            AmountPaid = AmountPaid,
                            Name = Name,
                            Street1 = Street1,
                            Street2 = Street2,
                            CityName = CityName,
                            StateOrProvince = StateOrProvince,
                            Country = Country,
                            PostalCode = PostalCode,
                            OrderDate = OrderDate,
                            OrderStatus = OrderStatus,
                            SellerID = SellerId,
                            CheckoutState = CheckoutState,
                            ShippedDate = ShippedDate,
                            SalesOrderNumber = SalesOrderNumber,
                            Channel = Channel,
                            SellerIndex = SellerIndex,
                            IsActive = IsActive,
                            OrderItems = lstOrderItemsAdd
                        });
                    }

                }
            }
            if (lstOrderAdd.Count() > 0)
            {
                InsertOrders(lstOrderAdd);
            }
            if (lstVmOrder.Count() > 0)
            {
                _listingRepository.UpdateOrderList(lstVmOrder);
            }
            if (lstVmOrderItems.Count() > 0)
            {
                _listingRepository.UpdateOrdersListFromExcelAmazon(lstVmOrderItems);
            }
            return true;
        }

        private List<int> GetItemMasterIdListBySKUId(List<string> referenceSkuList, List<SellerItemLink> lstSellerItemLink)
        {
            List<int> lstItemMasterId = lstSellerItemLink.Where(x => referenceSkuList.Contains(x.SKU)).Select(x => x.ItemMasterID).ToList();
            return lstItemMasterId;
        }


        private void LoadInfoForUsa(ref VM_OrderSheet_Upload vmOrderSheetConvertedData, DataRow item)
        {
            vmOrderSheetConvertedData.DateTime = getDate(item[0].ToString(), item[7].ToString()); //DateTime.Parse(item[0].ToString().Replace("UTC", "-0"));
            vmOrderSheetConvertedData.SettlementId = item[1].ToString();
            vmOrderSheetConvertedData.Type = item[2].ToString();

            vmOrderSheetConvertedData.OrderReferenceNo = item[3].ToString();

            vmOrderSheetConvertedData.SKU = item[4].ToString();
            vmOrderSheetConvertedData.Description = item[5].ToString();
            vmOrderSheetConvertedData.Quantity = int.Parse(item[6].ToString());


            vmOrderSheetConvertedData.MarketPlace = item[7].ToString();


            vmOrderSheetConvertedData.Fulfilment = item[9].ToString();
            vmOrderSheetConvertedData.OrderCity = item[10].ToString();
            vmOrderSheetConvertedData.OrderState = item[11].ToString();
            vmOrderSheetConvertedData.OrderPostal = item[12].ToString();


            vmOrderSheetConvertedData.TaxCollectionModel = item[13].ToString();
            vmOrderSheetConvertedData.ProductSalesPrice = Convert.ToDouble(item[14]);
            vmOrderSheetConvertedData.ProductSalesTax = Convert.ToDouble(item[15]);
            vmOrderSheetConvertedData.PostageCredits = Convert.ToDouble(item[16]);
            vmOrderSheetConvertedData.ShippingCreditsTax = Convert.ToDouble(item[17]);
            vmOrderSheetConvertedData.GiftWrapCredits = Convert.ToDouble(item[18]);
            vmOrderSheetConvertedData.GiftWrapCreditsTax = Convert.ToDouble(item[19]);
            vmOrderSheetConvertedData.PromotionalRebates = Convert.ToDouble(item[20]);
            vmOrderSheetConvertedData.PromotionalRebatesTax = Convert.ToDouble(item[21]);
            vmOrderSheetConvertedData.MarketplaceWithHeldTax = Convert.ToDouble(item[22]);
            vmOrderSheetConvertedData.SellingFees = Convert.ToDouble(item[23]);
            vmOrderSheetConvertedData.FbaFees = Convert.ToDouble(item[24]);
            vmOrderSheetConvertedData.OtherTransactionFees = Convert.ToDouble(item[25]);
            vmOrderSheetConvertedData.Other = Convert.ToDouble(item[26]);

            vmOrderSheetConvertedData.total = Convert.ToDouble(item[27]);
        }
        private void LoadInfoForTurkey(ref VM_OrderSheet_Upload vmOrderSheetConvertedData, DataRow item)
        {
            vmOrderSheetConvertedData.DateTime = getDate(item[0].ToString(), item[7].ToString());
            vmOrderSheetConvertedData.SettlementId = item[1].ToString();
            vmOrderSheetConvertedData.Type = item[2].ToString();

            vmOrderSheetConvertedData.OrderReferenceNo = item[3].ToString();

            vmOrderSheetConvertedData.SKU = item[4].ToString();
            vmOrderSheetConvertedData.Description = item[5].ToString();
            vmOrderSheetConvertedData.Quantity = int.Parse(item[6].ToString());


            vmOrderSheetConvertedData.MarketPlace = item[7].ToString();


            vmOrderSheetConvertedData.Fulfilment = item[8].ToString();
            vmOrderSheetConvertedData.OrderCity = item[9].ToString();
            vmOrderSheetConvertedData.OrderState = item[10].ToString();
            vmOrderSheetConvertedData.OrderPostal = item[11].ToString();


            //vmOrderSheetConvertedData.TaxCollectionModel = item[13].ToString();
            vmOrderSheetConvertedData.ProductSalesPrice = Convert.ToDouble(item[12]);
            //vmOrderSheetConvertedData.ProductSalesTax = Convert.ToDouble(item[15]);
            //vmOrderSheetConvertedData.PostageCredits = Convert.ToDouble(item[16]);
            vmOrderSheetConvertedData.ShippingCreditsTax = Convert.ToDouble(item[13]);
            //vmOrderSheetConvertedData.GiftWrapCredits = Convert.ToDouble(item[18]);
            //vmOrderSheetConvertedData.GiftWrapCreditsTax = Convert.ToDouble(item[19]);
            vmOrderSheetConvertedData.PromotionalRebates = Convert.ToDouble(item[14]);
            //vmOrderSheetConvertedData.PromotionalRebatesTax = Convert.ToDouble(item[21]);
            //vmOrderSheetConvertedData.MarketplaceWithHeldTax = Convert.ToDouble(item[22]);
            vmOrderSheetConvertedData.SellingFees = Convert.ToDouble(item[15]);
            vmOrderSheetConvertedData.FbaFees = Convert.ToDouble(item[16]);
            vmOrderSheetConvertedData.OtherTransactionFees = Convert.ToDouble(item[17]);
            vmOrderSheetConvertedData.Other = Convert.ToDouble(item[18]);

            vmOrderSheetConvertedData.total = Convert.ToDouble(item[19]);
        }

        private void LoadInfoForAu(ref VM_OrderSheet_Upload vmOrderSheetConvertedData, DataRow item)
        {
            vmOrderSheetConvertedData.DateTime = getDate(item[0].ToString(), item[7].ToString());
            vmOrderSheetConvertedData.SettlementId = item[1].ToString();
            vmOrderSheetConvertedData.Type = item[2].ToString();

            vmOrderSheetConvertedData.OrderReferenceNo = item[3].ToString();

            vmOrderSheetConvertedData.SKU = item[4].ToString();
            vmOrderSheetConvertedData.Description = item[5].ToString();
            vmOrderSheetConvertedData.Quantity = int.Parse(item[6].ToString());


            vmOrderSheetConvertedData.MarketPlace = item[7].ToString();


            vmOrderSheetConvertedData.Fulfilment = item[8].ToString();
            vmOrderSheetConvertedData.OrderCity = item[9].ToString();
            vmOrderSheetConvertedData.OrderState = item[10].ToString();
            vmOrderSheetConvertedData.OrderPostal = item[11].ToString();


            //vmOrderSheetConvertedData.TaxCollectionModel = item[12].ToString();
            vmOrderSheetConvertedData.ProductSalesPrice = Convert.ToDouble(item[12]);
            vmOrderSheetConvertedData.ProductSalesTax = Convert.ToDouble(item[16]);
            vmOrderSheetConvertedData.PostageCredits = Convert.ToDouble(item[13]);
            //vmOrderSheetConvertedData.ShippingCreditsTax = Convert.ToDouble(item[16]);
            vmOrderSheetConvertedData.GiftWrapCredits = Convert.ToDouble(item[14]);
            //try {
            //    var aa = Convert.ToDouble(item[15].ToString());
            //    vmOrderSheetConvertedData.PromotionalRebates =  Convert.ToDouble(item[15]);
            //}
            //catch (Exception ex)
            //{ 

            //}
            //var a = item[15];
            //vmOrderSheetConvertedData.GiftWrapCreditsTax = Convert.ToDouble(item[18]);
            vmOrderSheetConvertedData.PromotionalRebates = Convert.ToDouble(item[15]);
            //vmOrderSheetConvertedData.PromotionalRebatesTax = Convert.ToDouble(item[20]);
            //vmOrderSheetConvertedData.MarketplaceWithHeldTax = Convert.ToDouble(item[21]);
            vmOrderSheetConvertedData.SellingFees = Convert.ToDouble(item[18]);
            vmOrderSheetConvertedData.FbaFees = Convert.ToDouble(item[19]);
            vmOrderSheetConvertedData.OtherTransactionFees = Convert.ToDouble(item[20]);
            vmOrderSheetConvertedData.Other = Convert.ToDouble(item[21]);

            vmOrderSheetConvertedData.total = Convert.ToDouble(item[22]);
        }

        private void LoadInfoForUkDeItFr(ref VM_OrderSheet_Upload vmOrderSheetConvertedData, DataRow item)
        {
            //try {
            //    //DateTime oDate = DateTime.ParseExact(item[0].ToString(), "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            //    var a = DateTimeOffset.Parse("1 Jul 2021 1:04:20 am GMT+9");
            //}
            //catch (Exception ex)
            //{ 

            //}

            vmOrderSheetConvertedData.DateTime = getDate(item[0].ToString(), item[7].ToString());
            vmOrderSheetConvertedData.SettlementId = item[1].ToString();
            vmOrderSheetConvertedData.Type = item[2].ToString();

            vmOrderSheetConvertedData.OrderReferenceNo = item[3].ToString();

            vmOrderSheetConvertedData.SKU = item[4].ToString();
            vmOrderSheetConvertedData.Description = item[5].ToString();
            vmOrderSheetConvertedData.Quantity = int.Parse(item[6].ToString());


            vmOrderSheetConvertedData.MarketPlace = item[7].ToString();


            vmOrderSheetConvertedData.Fulfilment = item[8].ToString();
            vmOrderSheetConvertedData.OrderCity = item[9].ToString();
            vmOrderSheetConvertedData.OrderState = item[10].ToString();
            vmOrderSheetConvertedData.OrderPostal = item[11].ToString();


            vmOrderSheetConvertedData.TaxCollectionModel = item[12].ToString();
            vmOrderSheetConvertedData.ProductSalesPrice = Convert.ToDouble(item[13]);
            vmOrderSheetConvertedData.ProductSalesTax = Convert.ToDouble(item[14]);
            vmOrderSheetConvertedData.PostageCredits = Convert.ToDouble(item[15]);
            vmOrderSheetConvertedData.ShippingCreditsTax = Convert.ToDouble(item[16]);
            vmOrderSheetConvertedData.GiftWrapCredits = Convert.ToDouble(item[17]);
            vmOrderSheetConvertedData.GiftWrapCreditsTax = Convert.ToDouble(item[18]);
            vmOrderSheetConvertedData.PromotionalRebates = Convert.ToDouble(item[19]);
            vmOrderSheetConvertedData.PromotionalRebatesTax = Convert.ToDouble(item[20]);
            vmOrderSheetConvertedData.MarketplaceWithHeldTax = Convert.ToDouble(item[21]);
            vmOrderSheetConvertedData.SellingFees = Convert.ToDouble(item[22]);
            vmOrderSheetConvertedData.FbaFees = Convert.ToDouble(item[23]);
            vmOrderSheetConvertedData.OtherTransactionFees = Convert.ToDouble(item[24]);
            vmOrderSheetConvertedData.Other = Convert.ToDouble(item[25]);
            vmOrderSheetConvertedData.total = Convert.ToDouble(item[26]);
        }

        private DateTime getDate(string date, string marketPlace)
        {
            try
            {
                if (marketPlace.ToLower().Trim() == "amazon.co.uk")
                {
                    DateTime parseDate = DateTime.Parse(date.Replace("UTC", "-0"));
                    return parseDate;
                }
                //not working for date
                else if (marketPlace.ToLower().Trim() == "amazon.com.tr")
                {
                    date = date.Replace(",", "").Replace(".", "").Replace(" am", "").Replace(" pm", "").Trim();
                    DateTime parseDate = DateTime.ParseExact(date, "d MMM yyyy H:m:s", CultureInfo.InvariantCulture);
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.ca")
                {
                    date = date.Substring(0, date.LastIndexOf(" ")).Replace(",", "").Replace(".", "").Replace(" am", "").Replace(" pm", "").Trim();
                    DateTime parseDate = DateTime.ParseExact(date, "MMM d yyyy H:m:s", CultureInfo.InvariantCulture);
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.it")
                {
                    DateTime parseDate = DateTime.ParseExact(date.Replace(" UTC", ""), "d MMM yyyy HH:mm:ss", new CultureInfo("it-IT"));
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.fr")
                {
                    ////var a =        DateTime.Parse("1 août 2021 01:56:15 UTC", CultureInfo.GetCultureInfo("fr-FR"));

                    //       date = "1 août 2021 01:56:15 UTC".Replace(" UTC", "");
                    //       DateTime parseDate = DateTime.ParseExact(date, "d MMMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //       return parseDate;
                    DateTime parseDate = DateTime.Parse(date.Replace(" UTC", ""));
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.de")
                {
                    DateTime parseDate = DateTime.ParseExact(date.Replace(".", " ").Replace(" UTC", "").Trim(), "d MM yyyy HH:mm:ss", new CultureInfo("it-DE"));
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.es")
                {
                    //date = date.Replace(".", "").Replace("UTC", "").Replace(" AM", "").Replace(" PM", "").Trim();
                    //DateTime parseDate = DateTime.ParseExact(date, "d MMM yyyy H:m:s", CultureInfo.InvariantCulture);
                    //return parseDate;
                    DateTime parseDate = DateTime.Parse(date.Replace(" UTC", ""));
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.nl")
                {
                    DateTime parseDate = DateTime.ParseExact(date.Replace(" UTC", "").Replace(".", ""), "d MMM yyyy H:m:s", CultureInfo.InvariantCulture);
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.se")
                {
                    DateTime parseDate = DateTime.ParseExact(date.Replace(" UTC", "").Replace(".", ""), "d MMM yyyy H:m:s", CultureInfo.InvariantCulture);
                    return parseDate;
                }
                //not working for date
                else if (marketPlace.ToLower().Trim() == "amazon.pl")
                {
                    DateTime parseDate1 = DateTime.ParseExact("1 lis 2021", "d MMM yyyy", new CultureInfo("it-PL"));
                    date = date.Replace(" UTC", "").Replace(".", "");
                    DateTime parseDate = DateTime.ParseExact(date, "d MMM yyyy H:m:s", new CultureInfo("it-PL"));
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.com")
                {
                    date = date.Replace(",", "").Replace("PDT", "").Replace(" AM", "").Replace(" PM", "").Trim();
                    DateTime parseDate = DateTime.ParseExact(date, "MMM d yyyy H:m:s", CultureInfo.InvariantCulture);
                    return parseDate;
                }
                else if (marketPlace.ToLower().Trim() == "amazon.com.au")//need to check again.
                {
                    //DateTime dt = Convert.ToDateTime(date);
                    //string s = "Fri Nov 01 2013 00:00:00 GMT+0100";
                    //DateTime dt2 = DateTime.ParseExact(s, "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture);
                    date = date.Substring(0, date.LastIndexOf(" ")).Replace(",", "").Replace(" am", "").Replace(" pm", "").Trim();
                    date = date.Replace(",", "").Replace(" am", "").Replace(" pm", "").Trim();
                    //DateTime parseDate = DateTime.ParseExact(date, "d MMM yyyy H:m:s 'GMT'z", CultureInfo.InvariantCulture); 
                    DateTime parseDate = DateTime.ParseExact(date, "d MMM yyyy H:m:s", CultureInfo.InvariantCulture); 
                    return parseDate;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
            //vmOrderSheetConvertedData.DateTime = DateTime.Parse(item[0].ToString().Replace("UTC", "-0"));
        }

        private double GetActualDeliveryThisSKU(List<ItemMaster> lstItemMaster, List<SellerItemLink> lstSellerItemLink, List<PostalCarrier> lstPostalCarriers, VM_OrderSheet_Upload clientItem, string marketPlace)
        {
            double price = 0;
            SellerItemLink sellerItemLink = lstSellerItemLink.Where(x => x.SKU == clientItem.SKU).FirstOrDefault();
            if (sellerItemLink != null)
            {
                //ItemMaster itemMaster = lstItemMaster.Where(x => x.ItemMasterID == sellerItemLink.ItemMasterID).FirstOrDefault();
                //return itemMaster.ItemCost;
                if (!string.IsNullOrEmpty(sellerItemLink.Postage))
                {
                    PostalCarrier postalCarrier = lstPostalCarriers.Where(x => x.PostalCarrierID == int.Parse(sellerItemLink.Postage)).FirstOrDefault();
                    if (postalCarrier != null)
                    {
                        //if (marketPlace == "amazon.co.uk")
                        //{
                        //    price = postalCarrier.FBAUK;
                        //}
                        if (marketPlace.ToLower().Trim() == "amazon.co.uk")
                        {
                            price = postalCarrier.FBAUK;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.it")
                        {
                            price = postalCarrier.FBAIT;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.fr")
                        {
                            price = postalCarrier.FBAFR;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.de")
                        {
                            price = postalCarrier.FBADE;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.es")
                        {
                            price = postalCarrier.FBAES;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.nl")
                        {
                            price = postalCarrier.FBANL;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.se")
                        {
                            price = postalCarrier.FBASE;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.pl")
                        {
                            price = postalCarrier.FBAPL;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.com")
                        {
                            price = postalCarrier.FBAUSA;
                        }
                        else if (marketPlace.ToLower().Trim() == "amazon.com.au")
                        {
                            price = postalCarrier.FBAAU;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }


            }
            return price;
        }

        private double GetCostForThisSKUWithSellerId(List<ItemMaster> lstItemMaster, List<SellerItemLink> lstSellerItemLink, string sKU, List<ItemPrice> lstItemPriceList, ref string sellerId, ref int sellerIndex)
        {
            //var a = lstSellerItemLink.Where(x => x.SKU == sKU).ToList();
            SellerItemLink sellerItemLink = lstSellerItemLink.Where(x => x.SKU == sKU).FirstOrDefault();
            if (sellerItemLink != null)
            {
                sellerId = sellerItemLink.SellerID;
                sellerIndex = sellerItemLink.SellerIndex;
                //List<ItemPrice> lstitemPrice = lstItemPriceList.Where(x => x.ItemMasterID == sellerItemLink.ItemMasterID).ToList();
                ItemPrice itemPrice = lstItemPriceList.Where(x => x.ItemMasterID == sellerItemLink.ItemMasterID).FirstOrDefault();
                if (itemPrice != null)
                {
                    return itemPrice.Price;
                }
                else
                {
                    //ItemMaster itemMaster = lstItemMaster.Where(x => x.ItemMasterID == sellerItemLink.ItemMasterID).FirstOrDefault();
                    //return itemMaster.ItemCost;
                    return 0;
                }
            }
            return 0;
        }
        private double GetCostForThisSKU(List<ItemMaster> lstItemMaster, List<SellerItemLink> lstSellerItemLink, string sKU, List<ItemPrice> lstItemPriceList)
        {
            SellerItemLink sellerItemLink = lstSellerItemLink.Where(x => x.SKU == sKU).FirstOrDefault();
            if (sellerItemLink != null)
            {
                ItemPrice itemPrice = lstItemPriceList.Where(x => x.ItemMasterID == sellerItemLink.ItemMasterID).FirstOrDefault();
                if (itemPrice != null)
                {
                    return itemPrice.Price;
                }
                else
                {
                    //ItemMaster itemMaster = lstItemMaster.Where(x => x.ItemMasterID == sellerItemLink.ItemMasterID).FirstOrDefault();
                    //return itemMaster.ItemCost;
                    return 0;
                }
            }
            return 0;
        }

        private string GetCountryName(string marketPlace)
        {
            if (marketPlace.ToLower().Trim() == "amazon.co.uk")
            {
                return "GB";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.it")
            {
                return "IT";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.fr")
            {
                return "FR";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.de")
            {
                return "DE";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.es")
            {
                return "ES";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.nl")
            {
                return "NL";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.se")
            {
                return "SE";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.pl")
            {
                return "PL";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.com")
            {
                return "USA";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.com.au")
            {
                return "AU";
            }
            else
            {
                return "";
            }
        }
        private string GetCurrencyFrom(string marketPlace)
        {
            if (marketPlace.ToLower().Trim() == "amazon.co.uk")
            {
                return "GBP";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.it")
            {
                return "EURO";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.fr")
            {
                return "EURO";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.de")
            {
                return "EURO";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.es")
            {
                return "EURO";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.nl")
            {
                return "EURO";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.se")
            {
                return "SEK";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.pl")
            {
                return "PLN";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.com")
            {
                return "USD";
            }
            else if (marketPlace.ToLower().Trim() == "amazon.com.au")
            {
                return "AU$";
            }
            else
            {
                return "EURO";
            }
        }
    }
}
