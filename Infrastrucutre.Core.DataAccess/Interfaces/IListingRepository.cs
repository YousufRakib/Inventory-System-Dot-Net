using Infrastrucutre.Core.Models;
using Infrastrucutre.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
namespace Infrastrucutre.Core.DataAccess
{
    public interface IListingRepository
    {
        bool AddListing(Infrastrucutre.Core.Models.ListingRequest request,out int listingRequestID);
        bool FBARequest(Infrastrucutre.Core.Models.FbaRequest request, out int FRequestID);
        System.Collections.Generic.List<Infrastrucutre.Core.Models.ListingRequest> GetListingRequests(int requestID=0);
        System.Collections.Generic.List<Infrastrucutre.Core.Models.FbaRequest> GetFbaRequest(out int totalCount, int jtStartIndex = 0, int jtPageSize = 0);
        List<FBAPendingRequestSummary> GetFbaPendingRequestSummary();
        List<FbaRequest> GetFbaPendingRequest(int jtStartIndex = 0, int jtPageSize = 0,int SellerIndex = 0, int FBARootID = 0); //Get FBA Processing List
        List<FbaRequest> GetFbaSortedList(int jtStartIndex = 0, int jtPageSize = 0, int SellerIndex = 0, int FBARootID = 0); //Get FBA Sorted List
        bool UpdateFbaProcesingByID(int FBARequestID = 0, string status = "", string reason = ""); //ACCEPT Fba Request To Status is Active
        bool RejectFbaProcesingByID(int FBARequestID = 0); //REJECT FBA PROCESSING FORM STATUS IS Pendding
        List<ShipmentDetails> GetShipmentDetails(out int totalCount,int jtStartIndex = 0, int jtPageSize = 0);//Get Shipment Details 
        List<FbaRequest> GetShipmentHistory(string id, out int totalCount, int jtStartIndex = 0, int jtPageSize = 0); //Get Shipment History
        List<FbaRequest> GetFbaRequestHistory(string id, out int totalCount, int jtStartIndex = 0, int jtPageSize = 0); //Get FBA Request History
        List<FbaRequestViewModel> GetFbaRequestHistoryByFNSKU(string id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0);
        bool DeleteShipment(string ShipmentID);//Delete Shipment 
        List<ShipmentDetails> GetTotalFNSKU(string ShipmentID="");
        bool UpdateListing(Infrastrucutre.Core.Models.ListingRequest request);
        bool UpdateFbaRequest(Infrastrucutre.Core.Models.FbaRequest request);
        bool SubmitListing(ListingSubmission submission);
        ListingRequest GetListingRequestByID(int requestID);
        FbaRequest GetFbaRequestByID(int FBARequestID);//Get Details For Fba Request
        List<ListingRequestFilter> GetUnAllocatedListingRequestNo();
        List<ListingStatus> GetListingStatus();
        List<ListingChannel> GetListingChannels();
        ListingSubmission GetListingSubmissionByID(int submissionID);
        bool UpdateSubmission(ListingSubmission submission);
        List<Submission> GetListingSubmissions(int itemMasterID=0);
        bool InsertOrders(List<Order> orders);
        bool UpdateOrder(Order order);
        bool UpdateOrderList(List<VMOrder> orderList);
        List<ListingDocument> GetListingRequestImages(int requestID);
        bool Uploadfile(string fileName, string filePath, int requestID);
        bool DeleteFile(int requestID, int fileID);
        List<Order> GetListingOrders(int[] orderIDs = null, string startDate = null, string endDate = null, string sellerId = null, string dispatchStatus = null, string orderNumber = null, bool? specialDelivery = null, string jtSorting = null);
        bool CreateListingItems(List<BulkInsert> ItemsInserted);
        List<SellerAccount> GetSeller(string sellerID = "", bool ?sync = false);
        List<SellerAccount> GetSellerAccounts();
        bool UpdateCarrier(int PostalCarrierID, DateTime ProceedTime, int[] OrderIDs);
        bool updateReactiveDeletedOrders( DateTime ProceedTime, int[] OrderIDs);
        bool UpdateCarrierRef(string carrierRef, DateTime proceedTime, int[] orderIDs);
        bool CheckExistingOrder(string orderRefNo);
        List<ItemImage> GetItemCodes(string sellerID);
        bool InsertImages(List<ItemImage> images);
        bool UpdateOrders(List<OrderItem> orderItems);
        bool UpdateOrdersFromExcelAmazon(List<OrderItem> orderItems);
        bool UpdateOrdersListFromExcelAmazon(List<VMOrderItems> orderItems);
        bool UpdateOrderCarriers(List<Order> orders);
        bool InsertSalesCommission(SaleOrder sale, out string errorMessage);
        bool SynchronizeSKU();
        List<AutoCompleteItem> GetSellerList();

        List<AutoCompleteItem> GetPostageList(); //Get Postage For Sku New Recorde 
        SellerItemLink UpdateSellerItemLink(SellerItemLink link);
        List<SellerItemLink> GetSellerItemLink(out int rowCount, int id, string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0);
        List<SellerItemLink> GetItemSellerLink(int id);
        List<SellerItemLink> GetItemSellerLinkBySKU(List<string> lstSku);
        List<SellerItemLink> GetItemSellerLinkList();
        List<SellerItemLink> GetListingLinkSubmission(int id);
        SellerItemLink InsertLink(SellerItemLink link);
        bool UpdateLink(SellerItemLink link);
        bool DeleteLink(int ItemLinkId); //Delete SKU In Item Master 26-02-2021 Danish
        bool UpdateSellerAccount(SellerAccount account);
        SellerAccount InsertSellerAccount(SellerAccount account);
        bool UpdateSellerAuth(SellerAccount account);
        List<SellerItemLink> GetStockErrors();
        SellerItemLink InsertAndFix(SellerItemLink link);
        bool DeleteOrder(int orderID);
        bool RefreshOrderItems(string orderReferenceNo, List<OrderItem> items);
        bool UpdateAdditionalNotes(string addiotionalNotes, int[] orderIDs);
        //-------------------------------------------------------------------------------------------------------//
        List<ItemMaster> GetItemsList(string ItemMasterID = "");//Get Item List 15-10-2020 #Danish
        List<ItemMaster> GetSKUList(string ItemMasterID = "", bool? sync = false);//Get SKU List 15-10-2020 #Danish
        List<ItemMaster> GetAsinsList(string ItemMasterID = "");//Get Asin No List 15-10-2020 #Danish
        List<ItemMaster> GetAsinByID(string ItemMasterID = "");
        List<ItemMaster> GetSKUByID(string ListingItemNo = "");
        List<ItemMaster> GetUserItemByID(string UserID = ""); //User Item List 25-02-2021
        List<ItemMaster> GetItemsListByID(string SellerID = "");
        List<UserInformation> GetUser(string UserID = "", bool? sync = false);//Get Item List 03-02-2021 #Danish
        List<ItemDetails> GetFNSKUList(string ItemMasterID = "");//Get FNSKU List 15-10-2020 #Danish
        List<FBALocations> GetFBARootList(string FBARootID="",bool? sync = false); // Get FBA ROOT / Location of Warehouse
        List<UserInformation> GetGroupUser(string UserID = "", bool? sync = false);//Get Group User List 03-12-2021 #Danish
        
        //-------------------------------------------------28-10-2020--------------------------------------------//

        bool AddGroup(int GID, DateTime ProceedTime, int[] OrderIDs); //Group Update ----------10-02-2021
        bool AddShipment(string ShipmentID, string ShipmentMethod, string Destination,DateTime ProceedTime, decimal[] FinalQty, decimal[] FBABoxQty, int[] OrderIDs, string[] FNSKUs, int[] ItemMasterIDs, int[] FBARootIDs); //Shipment Update ----------20-03-2021


        Order GetOrderBySalesOrderId(string salesOrderId);
        List<Order> GetOrdersListByOrderId(List<int> lstOrderId);
        List<Order> GetOrdersListByOrderReferenceId(List<string> lstOrderReferenceId);
        List<int> GetItemMasterIdListBySKUId(List<string> lstSKUId);
        List<OrderItem> GetOrderItemsListByOrderId(List<int> lstOrderId);
        List<OrderItem> GetOrderItemsListByOrderId(int OrderId);
        List<OrderItem> GetOrderItemsListByOrderListId(List<int> OrderId);
        List<ItemPrice> GetItemPriceByItemMasterId(string fileFor, List<int> lstItemMasterList);
        string InsertFDBStockFileData(List<FDBStockFile> FDBStockFile);
    }
}
