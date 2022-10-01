using AkraTechFramework.Helpers;

using AkraTechFramework.Controllers;
using Infrastructure.Constants;
using Infrastructure.Core.Provider;
using Infrastrucutre.Core.DataAccess;
using Infrastrucutre.Core.Models;
using Infrastrucutre.Core.Models.ViewModels;
using InventoryManager.CustomFilters;
using RazorPDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.Data;
using Infrastrucutre.Constants;
using AT.Core.Common;

namespace InventoryManager.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        IItemProvider _itemProvider;
        IListingProvider _listingProvider;
        ICarrierProvider _carrierProvider;
        ISettingsProvider _settingsProvider;


        public ItemController(IItemProvider itemProvider, IListingProvider listingProvider, ICarrierProvider carrierProvider, ISettingsProvider settingsProvider)
        {
            this._itemProvider = itemProvider;
            this._listingProvider = listingProvider;
            this._carrierProvider = carrierProvider;
            this._settingsProvider = settingsProvider;

        }

        public ItemController()
            : this(new ItemProvider(new ItemRepository()), new ListingProvider(new ListingRepository()), new CarrierProvider(new CarrierRepository()), new SettingsProvider(new SettingsRepository()))
        {

        }

        public JsonResult KeepAlive()
        {
            return Json(MessageConstants.MESSAGE_SUCCESS, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StockManager()
        {
            loadInventoryLocation();
            loadStockSource();
            return View();
        }
        public ActionResult StockManagement()
        {
            loadInventoryLocation();
            loadStockSource();
            return View();
        }

        public ActionResult Source()
        {
            return View();
        }

        public ActionResult Stock()
        {
            loadSupplierList();
            return View();
        }

        public ActionResult ItemSearch(string ItemName)
        {
            ViewBag.ItemSearchString = ItemName;
            return View("Index");
        }

        public JsonResult GetSellers()
        {

            var list = _listingProvider.GetSellerList();

            var sellers = new List<dynamic>();

            foreach (var item in list)
            {
                sellers.Add(new { DisplayText = item.label, Value = item.id });
            }

            return Json(new { Result = "OK", Options = sellers });
        }

        public JsonResult GetPostage() // Get Postage  For SKU 24-02-2021
        {

            var list = _listingProvider.GetPostageList();

            var sellers = new List<dynamic>();

            foreach (var item in list)
            {
                sellers.Add(new { DisplayText = item.label, Value = item.id });
            }

            return Json(new { Result = "OK", Options = sellers });
        }


        public JsonResult GetItemsWithStock(string id, string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            int rowCount = 0;
            var items = _itemProvider.GetItemsWithStock(out rowCount, id, filter, filterText, jtStartIndex, jtPageSize);

            if (jtSorting != null)
            {
                string[] sort = jtSorting.Split(' ');

                if (sort[1].Equals("ASC", StringComparison.OrdinalIgnoreCase))
                {
                    if(sort[0]== "ItemName")
                    {
                        items = items.OrderBy(x => x.ItemName).ToList();
                    }
                    else if (sort[0] == "ItemCode")
                    {
                        items = items.OrderBy(x => x.ItemCode).ToList();
                    }
                    else if (sort[0] == "CostUk")
                    {
                        items = items.OrderBy(x => Convert.ToDouble(x.CostUk)).ToList();
                    }
                    else if (sort[0] == "UkWhStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.UkWhStock)).ToList();
                    }
                    else if (sort[0] == "UKFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.UKFBAStock)).ToList();
                    }
                    else if (sort[0] == "NEEZUKFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.NEEZUKFBAStock)).ToList();
                    }
                    else if (sort[0] == "UKTotalStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.UKTotalStock)).ToList();
                    }

                    else if (sort[0] == "UKStockLife")
                    {
                        items = items.OrderBy(x => int.Parse(x.UKStockLife)).ToList();
                    }
                    else if (sort[0] == "CostEU")
                    {
                        items = items.OrderBy(x => int.Parse(x.CostEU)).ToList();
                    }
                    else if (sort[0] == "EUWhStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.EUWhStock)).ToList();
                    }
                    else if (sort[0] == "EUFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.EUFBAStock)).ToList();
                    }
                    else if (sort[0] == "NEEZEUFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.NEEZEUFBAStock)).ToList();
                    }
                    else if (sort[0] == "CDisFBCStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.CDisFBCStock)).ToList();
                    }
                    else if (sort[0] == "EUTotalStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.EUTotalStock)).ToList();
                    }
                    else if (sort[0] == "EUStockLevel")
                    {
                        items = items.OrderBy(x => int.Parse(x.EUStockLevel)).ToList();
                    }
                    else if (sort[0] == "EUStockLife")
                    {
                        items = items.OrderBy(x => int.Parse(x.EUStockLife)).ToList();
                    }
                    else if (sort[0] == "CostUSA")
                    {
                        items = items.OrderBy(x => int.Parse(x.CostUSA)).ToList();
                    }
                    else if (sort[0] == "USAWhStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.USAWhStock)).ToList();
                    }
                    else if (sort[0] == "USAFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.USAFBAStock)).ToList();
                    }
                    else if (sort[0] == "USATotalStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.USATotalStock)).ToList();
                    }
                    else if (sort[0] == "USAStockLife")
                    {
                        items = items.OrderBy(x => int.Parse(x.USAStockLife)).ToList();
                    }
                    else if (sort[0] == "CostCA")
                    {
                        items = items.OrderBy(x => int.Parse(x.CostCA)).ToList();
                    }
                    else if (sort[0] == "CAWhStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.CAWhStock)).ToList();
                    }
                    else if (sort[0] == "CAFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.CAFBAStock)).ToList();
                    }
                    else if (sort[0] == "CATotalStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.CATotalStock)).ToList();
                    }
                    else if (sort[0] == "CAStockLevel")
                    {
                        items = items.OrderBy(x => int.Parse(x.CAStockLevel)).ToList();
                    }
                    else if (sort[0] == "CAStockLife")
                    {
                        items = items.OrderBy(x => int.Parse(x.CAStockLife)).ToList();
                    }

                    else if (sort[0] == "CostAU")
                    {
                        items = items.OrderBy(x => int.Parse(x.CostAU)).ToList();
                    }
                    else if (sort[0] == "AUWhStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.AUWhStock)).ToList();
                    }
                    else if (sort[0] == "AUFBAStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.AUFBAStock)).ToList();
                    }
                    else if (sort[0] == "AUTotalStock")
                    {
                        items = items.OrderBy(x => int.Parse(x.AUTotalStock)).ToList();
                    }
                    else if (sort[0] == "AUStockLevel")
                    {
                        items = items.OrderBy(x => int.Parse(x.AUStockLevel)).ToList();
                    }
                    else if (sort[0] == "AUStockLife")
                    {
                        items = items.OrderBy(x => int.Parse(x.AUStockLife)).ToList();
                    }
                    // itemMasters = items
                    //.OrderBy(r => r.GetType().GetProperty(sort[0].ToString()).GetValue(r, null)).ToList();
                }
                else
                {
                    if (sort[0] == "ItemName")
                    {
                        items = items.OrderByDescending(x => x.ItemName).ToList();
                    }
                    else if (sort[0] == "ItemCode")
                    {
                        items = items.OrderByDescending(x => x.ItemCode).ToList();
                    }
                    else if (sort[0] == "CostUk")
                    {
                        items = items.OrderByDescending(x => Convert.ToDouble(x.CostUk)).ToList();
                    }
                    else if (sort[0] == "UkWhStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.UkWhStock)).ToList();
                    }
                    else if (sort[0] == "UKFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.UKFBAStock)).ToList();
                    }
                    else if (sort[0] == "NEEZUKFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.NEEZUKFBAStock)).ToList();
                    }
                    else if (sort[0] == "UKTotalStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.UKTotalStock)).ToList();
                    }


                    else if (sort[0] == "UKStockLife")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.UKStockLife)).ToList();
                    }
                    else if (sort[0] == "CostEU")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CostEU)).ToList();
                    }
                    else if (sort[0] == "EUWhStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.EUWhStock)).ToList();
                    }
                    else if (sort[0] == "EUFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.EUFBAStock)).ToList();
                    }
                    else if (sort[0] == "NEEZEUFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.NEEZEUFBAStock)).ToList();
                    }
                    else if (sort[0] == "CDisFBCStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CDisFBCStock)).ToList();
                    }
                    else if (sort[0] == "EUTotalStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.EUTotalStock)).ToList();
                    }
                    else if (sort[0] == "EUStockLevel")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.EUStockLevel)).ToList();
                    }
                    else if (sort[0] == "EUStockLife")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.EUStockLife)).ToList();
                    }
                    else if (sort[0] == "CostUSA")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CostUSA)).ToList();
                    }
                    else if (sort[0] == "USAWhStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.USAWhStock)).ToList();
                    }
                    else if (sort[0] == "USAFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.USAFBAStock)).ToList();
                    }
                    else if (sort[0] == "USATotalStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.USATotalStock)).ToList();
                    }
                    else if (sort[0] == "USAStockLife")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.USAStockLife)).ToList();
                    }
                    else if (sort[0] == "CostCA")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CostCA)).ToList();
                    }
                    else if (sort[0] == "CAWhStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CAWhStock)).ToList();
                    }
                    else if (sort[0] == "CAFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CAFBAStock)).ToList();
                    }
                    else if (sort[0] == "CATotalStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CATotalStock)).ToList();
                    }
                    else if (sort[0] == "CAStockLevel")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CAStockLevel)).ToList();
                    }
                    else if (sort[0] == "CAStockLife")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CAStockLife)).ToList();
                    }

                    else if (sort[0] == "CostAU")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.CostAU)).ToList();
                    }
                    else if (sort[0] == "AUWhStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.AUWhStock)).ToList();
                    }
                    else if (sort[0] == "AUFBAStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.AUFBAStock)).ToList();
                    }
                    else if (sort[0] == "AUTotalStock")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.AUTotalStock)).ToList();
                    }
                    else if (sort[0] == "AUStockLevel")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.AUStockLevel)).ToList();
                    }
                    else if (sort[0] == "AUStockLife")
                    {
                        items = items.OrderByDescending(x => int.Parse(x.AUStockLife)).ToList();
                    }
                    //itemMasters = items
                    //        .OrderByDescending(r => r.GetType().GetProperty(sort[0].ToString()).GetValue(r, null)).ToList();
                }

            }
            return Json(new { Result = "OK", Records =items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }
        public JsonResult GetItemsWithAllStock(string id, string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _itemProvider.GetItemsWithStock(out rowCount, id, filter, filterText, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }



        public JsonResult GetItems(string id, string filter = "", string filterText = "", int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _itemProvider.GetItems(out rowCount, id, filter, filterText, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public JsonResult GetStockHistory(int id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _itemProvider.GetStockHistory(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public JsonResult GetFDBStockUploadHistory(int id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _itemProvider.GetFDBStockUploadHistory(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        [HttpGet]
        public JsonResult GetStockSource()
        {
            var items = _itemProvider.GetStockSource();

            return Json(new { Result = items }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ItemHistory(int id)
        {
            var items = _itemProvider.GetItemHistory(id);

            return View(items);
        }


        [HttpGet]
        public JsonResult LookupItem(string searchText, bool onlyItemName = false)
        {
            var items = _itemProvider.GetItemNames(searchText, onlyItemName);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LookupSupplier(string searchText)
        {
            var items = _itemProvider.GetSupplierNames(searchText);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemsBySupplier(int id = 0)
        {
            var items = _itemProvider.GetItemsBySupplier(id);
            return Json(new { Result = "OK", Records = items });
        }

        public ActionResult ItemCategory()
        {
            return View();
        }

        public ActionResult AmazonCategories()
        {
            return View();
        }

        public ActionResult EbayCategories()
        {
            return View();
        }

        public ActionResult ItemColor()
        {
            return View();
        }

        public ActionResult ItemLocation()
        {
            return View();
        }

        public ActionResult ItemManufacturer()
        {
            return View();
        }

        public ActionResult PostalCarrier()
        {
            return View();
        }

        public JsonResult GetItemCategories()
        {
            var items = _itemProvider.GetCategories();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetItemColors()
        {
            var colors = _itemProvider.GetColors();
            return Json(new { Result = "OK", Records = colors });
        }

        public JsonResult GetEbayItemCategories()
        {
            var items = _itemProvider.GetEbayCategories();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetAmazonItemCategories()
        {
            var items = _itemProvider.GetAmazonCategories();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetCarriers()
        {
            var items = _carrierProvider.GetCarriers();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetItemLocations()
        {
            var items = _itemProvider.GetLocations();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetItemManufacturers()
        {
            var items = _itemProvider.GetManufacturers();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetStockOut()
        {
            var items = _itemProvider.GetStockOut();
            return Json(new { Result = "OK", Records = items });
        }


        public JsonResult GetItemCategoryByID(int id)
        {
            var item = _itemProvider.GetCategoryByID(id);
            return Json(item);
        }

        public JsonResult GetEbayItemCategoryByID(int id)
        {
            var item = _itemProvider.GetEbayCategoryByID(id);
            return Json(item);
        }

        public JsonResult GetAmazonItemCategoryByID(int id)
        {
            var item = _itemProvider.GetAmazonCategoryByID(id);
            return Json(item);
        }

        public JsonResult GetItemColorByID(int id)
        {
            var item = _itemProvider.GetItemColorByID(id);
            return Json(item);
        }

        public JsonResult GetCarrierByID(int id)
        {
            var item = _carrierProvider.GetCarrierByID(id);
            return Json(item);
        }

        //--------------------------Delete Carrier Record 28-01-2021  ---- ---------------//

        public JsonResult DeleteCarrierByID(int PostalCarrierID)
        {
            if (_carrierProvider.DeleteCarrierByID(PostalCarrierID))
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });

        }

        //-------------------------------------------------------------------------------//

        public ActionResult GroupAssign(string ItemName) // Group Controller for Assigned Item to the User 03-02-2021 
        {
            ViewBag.ItemSearchString = ItemName;
            loadUsersList();
            loadItemList();
            return View();
        }

        private void loadUsersList() //User List
        {
            var User = _listingProvider.GetUser();
            ViewBag.User = new SelectList(User, "UserID", "UserName");

        }

        private void loadItemList() //Items List
        {
            var Item = _listingProvider.GetItemsList();
            ViewBag.Item = new SelectList(Item, "ItemMasterID", "ItemName");

        }


        //-----------------------------------------------------------------------//

        public JsonResult GetItemLocationByID(int id)
        {
            var item = _itemProvider.GetItemLocationByID(id);
            return Json(item);
        }

        public JsonResult GetItemManufacturerByID(int id)
        {
            var item = _itemProvider.GetItemManufacturerByID(id);
            return Json(item);
        }

        public ActionResult AddItems()
        {
            loadSupplierList();
            loadCategoryList();
            loadColorList();
            loadManufacturerList();
            loadLocationList();
            ViewBag.Mode = "Create";
            return View();
        }

        //_________________________________FBA Request _____________________//

        public JsonResult GetFbaRequest(int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _listingProvider.GetFbaRequest(out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });

        }


        public JsonResult GetFbaPendingRequest(int jtStartIndex = 0, int jtPageSize = 0, int SellerIndex = 0, int FBARootID = 0) // Report
        {
            try
            {
                int rowCount = 0;
                var items = _listingProvider.GetFbaPendingRequest(jtStartIndex, jtPageSize, SellerIndex, FBARootID);
                return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });

            }
            catch { return Json(new { Result = "ERROR", Message = "UnExpected Error" }); }
        }

        public JsonResult GetFbaPendingRequestSummary(int jtStartIndex = 0, int jtPageSize = 0, int SellerIndex = 0, int FBARootID = 0) // Report
        {
            try
            {
                //int rowCount = 0;
                var items = _listingProvider.GetFbaPendingRequestSummary();
                return Json(new { Result = "OK", Records = items});

            }
            catch { return Json(new { Result = "ERROR", Message = "UnExpected Error" }); }
        }


        public JsonResult UpdateProcesingByID(int fbaRequestID,string status,string reason) //Update FBA Pending Form to Sorted List by Accept 
        {
            bool items = _listingProvider.UpdateFbaProcesingByID(fbaRequestID,status,reason);

            if (items)
            {
                //Success("Process Sorted Successfully");
                return Json(new { Result = "OK" });

            }
            else
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });

        }

        public JsonResult RejectFbaProcesingByID(int id) //Update Fba Pending 
        {
            bool ISRejected = _listingProvider.RejectFbaProcesingByID(id);


            if (ISRejected)
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });


        }
        //____________Get FBA SORTED LIST 15-03-2021 __________________________//
        public JsonResult GetFbaSortedList(int jtStartIndex = 0, int jtPageSize = 0, int SellerIndex = 0, int FBARootID = 0) // FBA Sorted List
        {
            try
            {
                int rowCount = 0;
                var items = _listingProvider.GetFbaSortedList(jtStartIndex, jtPageSize, SellerIndex, FBARootID);
                return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public ContentResult AddShipment(string ShipmentID, string ShipmentMethod, string Destination, decimal[] FinalQty, decimal[] FBABoxQty, int[] OrderIDs, string[] FNSKUs, int[] ItemMasterIDs, int[] FBARootIDs)
        {
            DateTime proceedDateTime = DateTime.Now;

            _listingProvider.AddShipment(ShipmentID, ShipmentMethod, Destination, proceedDateTime, FinalQty, FBABoxQty, OrderIDs,FNSKUs,ItemMasterIDs,FBARootIDs);
            return Content("OK");

        }
        //____________________________________________________--//

        public ActionResult ShipmentDetails() // Shipment Details it will be display all fba sorted list with shipment details 
        {
            return View();
        }

        public JsonResult GetShipmentDetails(int jtStartIndex = 0, int jtPageSize = 0)
        {
            try
            {
                int rowCount = 0;
                var items = _listingProvider.GetShipmentDetails(out rowCount, jtStartIndex, jtPageSize);
                return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }

        }

        public JsonResult GetShipmentHistory(string id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _listingProvider.GetShipmentHistory(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public JsonResult GetFbaRequestHistory(string id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _listingProvider.GetFbaRequestHistory(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public JsonResult GetFbaRequestHistoryByFNSKU(string id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _listingProvider.GetFbaRequestHistoryByFNSKU(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public ActionResult DeleteShipment(string ShipmentID = "")
        {


            bool IsDeletedSuccessfully = _listingProvider.DeleteShipment(ShipmentID);

            if (IsDeletedSuccessfully)
                Success("Group Deleted Successfully");
            else
                Error(MessageConstants.UNEXPECTED_ERROR);

            return RedirectToAction("ShipmentDetails");



        }

        public JsonResult GetTotalFNSKU(string ShipmentID = "")
        {
            var TotalFNSKU = _listingProvider.GetTotalFNSKU(ShipmentID);
            return Json(new { Result = "OK", Records = TotalFNSKU });

        }
        //_______________________________________________________________//
        public ActionResult FBARequestForm()//FBA Req Lists
        {
            loadUserList();
            loadSellersList();
            loadItemLists();
            loadAsinsList();
            loadSkuList();
            loadFnskuList();
            loadFbaRootList();

            ViewBag.HeaderText = "Add New Request";
            ViewBag.Mode = "Create";
            return View();

        }
        public ActionResult FBARequest()
        {
            loadUserList();
            loadSellersList();
            loadItemLists();
            loadAsinsList();
            loadSkuList();
            loadFnskuList();
            loadFbaRootList();

            ViewBag.HeaderText = "Add New Request";
            ViewBag.Mode = "Create";
            return View();
        }

        public ActionResult FBAProcessing() //FBA PROCESSING FORM 15-03-2021
        {
            loadUserList();
            loadSellersList();
            loadFbaRootList();
            return View();
        }

        public ActionResult FBASortedList() //FBA Sorted List 15-03-2021
        {
            loadUserList();
            loadSellersList();
            loadFbaRootList();
            return View();
        }

        public void loadFbaRootList(object selectedItem = null) //FBA Root List
        {
            var FBARoot = _listingProvider.GetFBARootList();
            ViewBag.FBARoot = new SelectList(FBARoot, "FBARootID", "FBARoot", selectedItem);

        }
        public void loadFnskuList(object selectedItem = null) //FNSKU List
        {
            var FNSKU = _listingProvider.GetFNSKUList();
            ViewBag.FNSKU = new SelectList(FNSKU, "ItemMasterID", "FNSKU", selectedItem);

        }
        private void loadUserList(object selectedItem = null)
        {

            var UserID = _listingProvider.GetUser();

            ViewBag.UserID = new SelectList(UserID, "UserID", "UserName", selectedItem);
        }

        private void loadSellersList(object selectedItem = null)
        {
            var sellers = _listingProvider.GetSeller();
            ViewBag.SellerID = new SelectList(sellers, "SellerIndex", "SellerID", selectedItem);
        }

        public void loadSkuList(object selectedItem = null) //SKU List
        {
            var SKU = _listingProvider.GetSKUList();
            ViewBag.SKU = new SelectList(SKU, "ItemMasterID", "SKU", selectedItem);

        }

        private void loadAsinsList(object selectedItem = null) //Get Asin List 
        {
            var Asin = _listingProvider.GetAsinsList();
            ViewBag.StockId = new SelectList(Asin, "ListingItemNo", "ListingItemNo", selectedItem);
        }

        private void loadItemLists(object selectedItem = null)
        {
            var items = _itemProvider.GetItemNames();

            ViewBag.ItemMasterID = new SelectList(items, "ItemMasterID", "ItemName", selectedItem);
        }

        public JsonResult GetASINListById(int ItemMasterID) // Get Item List By User Group 25-02-2021
        {

            var AsinList = _itemProvider.GetAsinByID(ItemMasterID).Where(x => x.ItemMasterID == ItemMasterID).Select(x => new
            {
                ItemMasterID = x.ItemMasterID,
                ListingItemNo = x.ListingItemNo
            }).ToList();

            AsinList.Insert(0, new { ItemMasterID = 0, ListingItemNo = "SELECT" });

            return Json(AsinList, JsonRequestBehavior.AllowGet);



        }

        public JsonResult GetSKUListById(string ListingItemNo)
        {

            var SKUList = _itemProvider.GetSKUByID(ListingItemNo).Where(x => x.ListingItemNo == ListingItemNo).Select(x => new
            {
                ListingItemNo = x.ListingItemNo.ToString(),
                SKU = x.SKU.ToString()
            }).ToList();

            SKUList.Insert(0, new { ListingItemNo = "", SKU = "SELECT" });
            return Json(SKUList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetUserItemListById(int UserID) // Get Item List By User 25-02-2021
        {

            var UserList = _itemProvider.GetUserItemByID(UserID).Where(x => x.UserID == UserID).Select(x => new
            {
                ItemMasterID = x.ItemMasterID,
                ItemName = x.ItemName
            }).ToList();

            UserList.Insert(0, new { ItemMasterID = 0, ItemName = "SELECT" });
            return Json(UserList, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetFNSKUListById(string SKU) //Get FNSKU  BY Item
        {
            var FNSKU = _itemProvider.GetFNSKUListByID(SKU).Where(x => x.SKU == SKU).Select(x => new
            {
                SKU = x.SKU,
                FNSKU = x.FNSKU
            }).ToList();

            FNSKU.Insert(0, new { SKU = "", FNSKU = "SELECT" });
            return Json(FNSKU, JsonRequestBehavior.AllowGet);
        }

        //__________________________________________________________________________//

        [HttpPost]
        public ActionResult QuickUpdate(ItemMaster item)
        {
            bool IsUpdated = _itemProvider.QuickUpdate(item);
            if (IsUpdated)
            {
                return Json(new { Result = "OK" });
            }
            else
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });
            }
        }

        [HttpPost]
        public ActionResult UpdateStockOut(StockOut option)
        {
            var stock = _itemProvider.UpdateStockOut(option);

            if (stock)
            {
                return Json(new { Result = "OK" });
            }
            else
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });
            }
        }

        [HttpPost]
        public ActionResult AddStockOut(StockOut option)
        {
            var stock = _itemProvider.AddStockOut(option);

            return Json(new { Result = "OK", Record = stock });
        }

        public class UpdateStockIn
        {
            public int FBARootID { get; set; }
            public string StockType { get; set; }
            public int InSource { get; set; }
            public int OutSource { get; set; }
            public string Notes { get; set; }
            public int ItemMasterId { get; set; }
            public int quantity { get; set; }
            
            public string BatchNo { get; set; }
            public string FnSku { get; set; }

        }

        //FBARootID=1&StockType=OUT&InSource=1&OutSource=4&notes=asdsadsa&ItemMasterId=11795"
        [HttpPost]
        public ActionResult UpdateStockFromStockManagement(UpdateStockIn updateStock)
        { 
            ItemStock item = new ItemStock();
            item.FBARootID = updateStock.FBARootID;//
            item.ItemMasterID = updateStock.ItemMasterId;
            item.InSource = updateStock.InSource;//
            item.OutSource = updateStock.OutSource;//
            item.Notes = updateStock.Notes;//
            item.FnSku = updateStock.FnSku;
            item.BatchNo = updateStock.BatchNo;//
            
            if (updateStock.StockType == "IN")
            {
                item.OutSource = -1;
                item.StockIn = updateStock.quantity;
                item.Type = (int)AppUtil.EntryType.StockEntry;
            }
            else
            {
                item.InSource = -1;
                item.StockOut = updateStock.quantity; 
                item.Type = (int)AppUtil.EntryType.StockShift;
            }

            var stock = _itemProvider.UpdateStockReturnStockManagementView(item);

            if (stock.IsUpdated)
            {
                return Json(new { Result = "OK", View = stock });
            }
            else
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });
            }
        }

        [HttpPost]
        public ActionResult UpdateStock(ItemStock item, int quantity, string stocktype)
        {
            if (stocktype == "IN")
            {
                item.OutSource = -1;
                item.StockIn = quantity;

            }
            else
            {
                item.InSource = -1;
                item.StockOut = quantity;

            }

            var stock = _itemProvider.UpdateStock(item);

            if (stock.IsUpdated)
            {
                return Json(new { Result = "OK", View = stock });
            }
            else
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });
            }
        }
         

        [HttpPost]
        public ActionResult GetStockNumber(VMSearchClass searchClass)
        {
            VMStockQuantityAndCost res = _itemProvider.GetStockQuantityByItemMasterIdFBARootIdItemCode(searchClass);
            //return Json(new { quantity = res!= null ? res.CurrentStock : 0 });
            return Json(new { data = res });
        }
        //_________________________________________FBA Request Form ________________________________//

        [HttpPost]
        public ActionResult AddFBARequest(FbaRequest item, string command)
        {
            bool IsUpdated = false;
            bool IsCreated = false;

            if (item.FNSKU == null)
            {
                Attention("Please select FNSKU from FNSKU list!!");
                return RedirectToAction("FBARequestForm");
            }
            else
            {
                if (command == "Save")
                {
                    IsCreated = _itemProvider.AddFBARequest(item);
                }
                else if (command == "Update")
                {
                    IsUpdated = _itemProvider.UpdateFBARequest(item, UserIdentity.UserID);  //_itemProvider.Updateitem(item, UserIdentity.UserID);
                }

                if (IsCreated)
                {
                    Success("FBA Request Created Successfully.");
                    return RedirectToAction("FBARequestForm");
                }
                else if (IsUpdated)
                {
                    Success("FBA Request Updated Successfully.");
                    return RedirectToAction("FBARequestForm");
                }
                else
                {
                    this.loadUserList(item.UserID);
                    this.loadSellersList(item.SellerIndex);
                    this.loadItemLists(item.ItemMasterID);
                    this.loadAsinsList(item.ListingItemNo);
                    this.loadSkuList(item.SKU);
                    //this.loadItemLists();
                    this.loadFnskuList(item.FNSKU);
                    this.loadFbaRootList(item.FBARootID);

                    Attention("Please verify the fields you have entered");
                    return View(item);
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteFBARequest(int FBARequestID)
        {
            if (_itemProvider.DeleteFBARequest(FBARequestID))
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "ERROR", Message = "UnExpected Error" });

        }

        public ActionResult GetFbaRequestByID(int id) //Edit Recorde
        {


            FbaRequest request = _listingProvider.GetFbaRequestByID(id);

            loadUserList(request.UserID);
            loadSellersList(request.SellerIndex);
            loadItemLists(request.ItemMasterID);
            loadAsinsList(request.ListingItemNo);
            loadSkuList(request.SKU);
            //this.loadItemLists();
            loadFnskuList(request.FNSKU);
            loadFbaRootList(request.FBARootID);




            ViewBag.HeaderText = "Update FBA Requests";
            ViewBag.Mode = "Update";

            return View("FBARequestForm", request);

        }
        //__________________________________________________________________________________________//
        [HttpPost]
        public ActionResult AddItems(ItemMasterViewModel itemMasterViewModel, string command)
        {
            bool IsUpdated = false;
            bool IsCreated = false;

            ItemMaster item = new ItemMaster();
            item.ItemMasterID = itemMasterViewModel.ItemMasterID;
            item.ItemName = itemMasterViewModel.ItemName;
            item.ItemCode = itemMasterViewModel.ItemCode;
            item.Description = itemMasterViewModel.Description;
            item.Brand = itemMasterViewModel.Brand;
            item.Dimension = itemMasterViewModel.Dimension;
            item.ItemWeight = itemMasterViewModel.ItemWeight;
            item.VAT = itemMasterViewModel.VAT;
            item.ItemCost = itemMasterViewModel.ItemCost;
            item.Length = itemMasterViewModel.Length;
            item.Width = itemMasterViewModel.Width;
            item.Height = itemMasterViewModel.Height;
            //item.CBM3 = itemMasterViewModel.CBM3;
            item.LeadTime = itemMasterViewModel.LeadTime;
            item.BarCode = itemMasterViewModel.BarCode;
            item.FNSKU = itemMasterViewModel.FNSKU;
            item.Notes = itemMasterViewModel.Notes;
            item.SupplierID = itemMasterViewModel.SupplierID;
            item.ItemCategoryID = itemMasterViewModel.ItemCategoryID;
            item.ItemManufacturerID = itemMasterViewModel.ItemManufacturerID;
            item.ItemColorID = itemMasterViewModel.ItemColorID;
            item.LocationID = itemMasterViewModel.LocationID;
            item.MasterCartonQty = itemMasterViewModel.MasterCartonQty;
            item.ReOrderLevel = itemMasterViewModel.ReOrderLevel;
            item.DateOfSubmission = itemMasterViewModel.DateOfSubmission;
            item.CartonQty = itemMasterViewModel.CartonQty;

            if (command == "Save")
            {
                IsCreated = _itemProvider.AddItems(item);
            }
            else if (command == "Update")
            {
                IsUpdated = _itemProvider.UpdateItems(item, UserIdentity.UserID);
            }

            if (IsCreated)
            {
                Success("Item Created Successfully.");
                return RedirectToAction("Index");
            }
            else if (IsUpdated)
            {
                Success("Item Updated Successfully.");
                return RedirectToAction("Index");
            }
            else
            {
                loadSupplierList(item.SupplierID);
                loadCategoryList(item.ItemCategoryID);
                loadColorList(item.ItemColorID);
                loadManufacturerList(item.ItemManufacturerID);
                loadLocationList(item.LocationID);
                Attention("Please verify the fields you have entered");
                return View(itemMasterViewModel);
            }
        }

        [HttpPost]
        public JsonResult DeleteItem(int id)
        {
            if (IsAdmin)
            {
                if (_itemProvider.DeleteItem(id))
                {
                    return Json(new { Result = "OK", Message = "Data remove successfully!!" });
                }

            }
            else
            {
                return Json(new { Result = "ERROR", Message = "You Are Not Admin!!" });
            }

            return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
        }


        public ActionResult GetItemByID(int id) //-----------------Test Its For Admin Or Not
        {
            ItemMasterViewModel item = new ItemMasterViewModel();
            ItemMaster itemData = _itemProvider.GetItemByID(id);

            if (itemData == null)
            {
                loadSupplierList();
                loadCategoryList();
                loadColorList();
                loadManufacturerList();
                loadLocationList();
            }
            else
            {
                item.ItemMasterID = itemData.ItemMasterID;
                item.ItemName = itemData.ItemName;
                item.ItemCode = itemData.ItemCode;
                item.Description = itemData.Description;
                item.Brand = itemData.Brand;
                item.Dimension = itemData.Dimension;
                item.ItemWeight = itemData.ItemWeight;
                item.VAT = itemData.VAT;
                item.ItemCost = itemData.ItemCost;
                item.Length = itemData.Length;
                item.Width = itemData.Width;
                item.Height = itemData.Height;
                //item.CBM3 = itemData.CBM3;
                item.LeadTime = itemData.LeadTime;
                item.BarCode = itemData.BarCode;
                item.FNSKU = itemData.FNSKU;
                item.Notes = itemData.Notes;
                item.SupplierID = itemData.SupplierID;
                item.ItemCategoryID = itemData.ItemCategoryID;
                item.ItemManufacturerID = itemData.ItemManufacturerID;
                item.ItemColorID = itemData.ItemColorID;
                item.LocationID = itemData.LocationID;
                item.MasterCartonQty = itemData.MasterCartonQty;
                item.ReOrderLevel = itemData.ReOrderLevel;
                item.DateOfSubmission = itemData.DateOfSubmission;
                item.CartonQty = itemData.CartonQty;

                loadSupplierList(item.SupplierID);
                loadCategoryList(item.ItemCategoryID);
                loadColorList(item.ItemColorID);
                loadManufacturerList(item.ItemManufacturerID);
                loadLocationList(item.LocationID);
            }
            
            ViewBag.HeaderText = "Update Item";
            if (!IsAdmin)
            {
                ViewBag.IsAdmin = IsAdmin;
            }

            return View("AddItems", item);
        }

        //public JsonResult GetItemsByID(int id)
        //{
        //    ItemMaster item = _itemProvider.GetItemInfoByID(id);

        //    return Json(item);

        //}
        public JsonResult GetsItemsByID(int itemId, int sellerId)
        {
            ItemMaster item = _itemProvider.GetsItemsByID(itemId, sellerId);
            return Json(item);

        }

        public JsonResult GetFBAStockByFNSKU(string SKU, int sellerId)
        {
            ItemMaster item = _itemProvider.GetFBAStockByFNSKU(SKU, sellerId);
            return Json(item);

        }

        private DataTable GetOrderData(List<SupplierOrder> orders)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = "SupplierOrder";

            //Add Columns  
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("Item Name", typeof(string));
            dt.Columns.Add("Item Cost", typeof(float));
            dt.Columns.Add("Supplier", typeof(string));
            dt.Columns.Add("Warehouse Stock", typeof(int));
            dt.Columns.Add("Unit/Carton", typeof(int));
            dt.Columns.Add("CBM", typeof(float));
            dt.Columns.Add("Carton QTY", typeof(int));
            dt.Columns.Add("Total Order Unit", typeof(int));
            dt.Columns.Add("Total CBM", typeof(float));
            dt.Columns.Add("FOB Price/Item", typeof(float));
            dt.Columns.Add("Total FOB Price", typeof(float));
            dt.Columns.Add("Notes", typeof(string));

            foreach (var item in orders)
            {
                dt.Rows.Add(item.ItemCode, item.ItemName, item.ItemCost,
                            item.SupplierName, item.CurrentStock, item.MasterCartonQty,
                            item.CBM, item.CartonQty, item.TotalOrderUnit, item.TotalCBM,
                            item.FOBPricePerItem, item.TotalFOBPrice, item.Notes);

                dt.AcceptChanges();
            }

            return dt;
        }

        private DataTable GetItemData(List<ItemMaster> items)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            //Setiing Table Name  
            dt.TableName = "StockItems";

            //Add Columns  
            dt.Columns.Add("ItemMasterID", typeof(int));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("Cost-UK(£)", typeof(string));
            dt.Columns.Add("UK-W.H_Stock", typeof(string));
            dt.Columns.Add("ARSUK-UK FBA Stock", typeof(string));
            dt.Columns.Add("NEEZ-UK FBA Stock", typeof(string));
            dt.Columns.Add("UK-Total Stock", typeof(string));
            //dt.Columns.Add("Stock Life Days", typeof(string));
            dt.Columns.Add("Cost-EURO", typeof(string));
            dt.Columns.Add("EU-W.H Stock", typeof(string));
            dt.Columns.Add("ARSUK-EU-FBA Stock", typeof(string));
            dt.Columns.Add("NEEZ-EU-FBA Stock", typeof(string));

            dt.Columns.Add("CDIS FBC Stock", typeof(string));
            dt.Columns.Add("EU-Total Stock", typeof(string));
            //dt.Columns.Add("Stock Level", typeof(string));
            //dt.Columns.Add("Stock Life Days", typeof(string));
            dt.Columns.Add("Cost-USA", typeof(string));
            dt.Columns.Add("USA-W.H_Stock", typeof(string));
            dt.Columns.Add("USA-FBA Stock", typeof(string));
            dt.Columns.Add("USA-TotalStock", typeof(string));
            //dt.Columns.Add("Stock Life Days", typeof(string));
            dt.Columns.Add("Cost-CA", typeof(string));
            dt.Columns.Add("CA-W.H-Stock", typeof(string));
            dt.Columns.Add("CA-FBA Stock", typeof(string));
            dt.Columns.Add("CA-Total Stock", typeof(string));

            //dt.Columns.Add("CA-Stock Level", typeof(string));
            //dt.Columns.Add("Stock Life Days", typeof(string));
            dt.Columns.Add("Cost-AU", typeof(string));
            dt.Columns.Add("AU-W.H_Stock", typeof(string));
            dt.Columns.Add("AU-FBA Stock", typeof(string));
            dt.Columns.Add("AU-Total Stock", typeof(string));
            //dt.Columns.Add("AU-Stock Level", typeof(string));
            //dt.Columns.Add("Stock Life Days", typeof(string));

            foreach (var item in items)
            {
                dt.Rows.Add(item.ItemMasterID, item.ItemName, item.ItemCode,
                            item.CostUk, item.UkWhStock, item.UKFBAStock,
                            item.NEEZUKFBAStock, item.UKTotalStock, /*item.UKStockLife,*/ item.CostEU,
                            item.EUWhStock, item.EUFBAStock, item.NEEZEUFBAStock,
                            item.CDisFBCStock, item.EUTotalStock,/* item.EUStockLevel,*/
                            /*item.EUStockLife,*/ item.CostUSA, item.USAWhStock,
                            item.USAFBAStock, item.USATotalStock, /*item.USAStockLife,*/ item.CostCA,
                            item.CAWhStock, item.CAFBAStock, item.CATotalStock,
                            /*item.CAStockLevel, item.CAStockLife,*/ item.CostAU,
                            item.AUWhStock, item.AUFBAStock, item.AUTotalStock
                            /*item.AUStockLevel, item.AUStockLife*/);

                dt.AcceptChanges();
            }

            return dt;
        }

        [HttpGet]
        public JsonResult GetItemByIDJson(int id)
        {
            ItemMaster item = _itemProvider.GetItemByID(id);

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DownloadOrder(List<SupplierOrder> orders)
        {
            DataTable dt = GetOrderData(orders);
            //Name of File  
            string fileName = "Sample.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                //wb.Worksheets.Add(dt);

                var sheet = wb.AddWorksheet("SupplierOrder");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateCarrierImage(HttpPostedFileBase file, int CarrierID)
        {
            if (file == null)
            {
                //_settingsProvider.UpdateSettings(string.Empty, string.Empty, companyName);
            }
            else if (file.ContentLength > 0)
            {
                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".jpeg" };

                int MaxContentLength = 1024 * 1024 * 3; //3 MB

                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
                {
                    Error("Please file of type: " + string.Join(", ", AllowedFileExtensions));
                }

                if (file.ContentLength > MaxContentLength)
                {
                    Error("Your file is too large, maximum allowed size is: " + 3 + " MB");
                }
                else
                {
                    try
                    {
                        Random randomNumberGenerator = new Random();
                        var fileName = string.Format("{0}_{1}", randomNumberGenerator.Next(1, 32000), Path.GetFileName(file.FileName));
                        var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                        file.SaveAs(path);
                        ModelState.Clear();
                        bool isSuccess = true;// _carrierProvider.UpdateCarrierImage(new PostalCarrier() { PostalCarrierID = CarrierID, CarrierImage = fileName });
                        if (isSuccess)
                        {
                            Success("File Uploaded Successfully");
                        }
                        else
                            Error(MessageConstants.UNEXPECTED_ERROR);
                    }
                    catch (Exception ex)
                    {
                        Error(MessageConstants.UNEXPECTED_ERROR);
                    }
                }



            }

            return RedirectToAction("PostalCarrier");

        }

        public ActionResult CreateListingLink(string selectedvalues)
        {
            if (selectedvalues.ElementAt(0) == ',')
                selectedvalues = selectedvalues.Remove(0, 1);

            int[] items = (selectedvalues.Split(',').Select(t => int.Parse(t)).ToArray());

            var list = new List<SellerItemLink>();

            var insertList = _itemProvider.CreateListings(items);

            foreach (var id in items)
            {
                var item = _itemProvider.GetItemsByID(id);

                list.Add(new SellerItemLink()
                {
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    ItemMasterID = item.ItemMasterID,
                });
            }

            ViewBag.SellerList = _listingProvider.GetSellerList();

            return View(list);

        }

        public ActionResult CreateListings(string selectedvalues)
        {

            if (selectedvalues.ElementAt(0) == ',')
                selectedvalues = selectedvalues.Remove(0, 1);

            int[] items = (selectedvalues.Split(',').Select(t => int.Parse(t)).ToArray());

            //int[] items = { 2, 4, 5 };


            var item = _itemProvider.CreateListings(items);
            ViewBag.EbayCategory = _itemProvider.GetEbayCategories();
            ViewBag.AmazonCategory = _itemProvider.GetAmazonCategories();
            ViewBag.ListingStatus = _listingProvider.GetListingStatus();
            ViewBag.ListingChannelID = _listingProvider.GetListingChannels();
            return View(item);
        }

        public ActionResult CreateOrder()
        {
            var list = new List<SupplierOrder>();

            var item = new SupplierOrder();

            list.Add(item);

            return View(list);
        }

        public JsonResult CheckUser() //Update Fba Pending 
        {
            string userId = User.Identity.ToGenericUserIdentity().Name;

            if (userId== "admin")
            {
                return Json(new { Result = "OK", Message = "Data remove successfully!!" });
            }
            else if (userId == "alraze")
            {
                return Json(new { Result = "OK", Message = "Data remove successfully!!" });
            }
            else
            {
                return Json(new { Result = "ERROR", Message = "Only admin can download file!!" });
            }
        }

        public ActionResult ExportToExcel(string filter = "", string filterText = "")
        {
            int rowCount = 0;
            var items = _itemProvider.GetItemsWithStock(out rowCount, "", filter, filterText, 0, 1000000);

            DataTable dt = GetItemData(items);
            //Name of File  
            string fileName = string.Format("ItemsStock-{0}.xlsx", DateTime.Now.ToString("yyyyMMdd-HHmmss"));

            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet
                wb.Worksheets.Add(dt);

                var sheet = wb.Worksheets.FirstOrDefault();

                sheet.Row(1).InsertRowsAbove(3);

                sheet.Cell(1, 2).Style.Font.Bold = true;
                sheet.Cell(1, 3).Style.Font.Bold = true;
                sheet.Cell(2, 2).Style.Font.Bold = true;
                sheet.Cell(2, 3).Style.Font.Bold = true;

                sheet.Cell(1, 2).Value = "Total Items: ";
                sheet.Cell(1, 3).Value = items.Count;
                sheet.Cell(2, 2).Value = "Export Date: ";
                sheet.Cell(2, 3).Value = DateTime.Now.ToString("dd/MM/yyyy");
               
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }

            }
        }

        [HttpPost]
        public ActionResult AddOrderItem(List<SupplierOrder> ItemsInserted, string orderNumber, string orderDate, string mode)
        {
            if (mode == "add")
            {
                var list = new List<SupplierOrder>(ItemsInserted);

                var item = new SupplierOrder();

                list.Add(item);

                ViewBag.orderNumber = orderNumber;
                if (!string.IsNullOrWhiteSpace(orderDate))
                {
                    DateTime orderDateObj;

                    if (DateTime.TryParse(orderDate, out orderDateObj))
                    {
                        ViewBag.orderDate = string.Format("{0:MM/dd/yyyy}", orderDateObj);
                    }
                }

                return View("CreateOrder", list);
            }
            else
            {
                if (ItemsInserted != null)
                {
                    DataTable dt = GetOrderData(ItemsInserted);
                    //Name of File  
                    string fileName = string.Format("SupplierOrder-{0}.xlsx", DateTime.Now.ToString("yyyyMMdd-HHmmss"));

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        //Add DataTable in worksheet  
                        wb.Worksheets.Add(dt);

                        var sheet = wb.Worksheets.FirstOrDefault();

                        sheet.Row(1).InsertRowsAbove(3);

                        sheet.Cell(1, 2).Style.Font.Bold = true;
                        sheet.Cell(1, 3).Style.Font.Bold = true;
                        sheet.Cell(2, 2).Style.Font.Bold = true;
                        sheet.Cell(2, 3).Style.Font.Bold = true;

                        sheet.Cell(1, 2).Value = "Order Number#";
                        sheet.Cell(1, 3).Value = orderNumber;
                        sheet.Cell(2, 2).Value = "Order Date";
                        if (!string.IsNullOrWhiteSpace(orderDate))
                        {
                            //return string.Format("{0:dd/MM/yyyy}", this.OrderDate);

                            DateTime orderDateObj;

                            if (DateTime.TryParse(orderDate, out orderDateObj))
                            {
                                sheet.Cell(2, 3).Value = string.Format("{0:dd/MM/yyyy}", orderDateObj);
                            }
                        }



                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            //Return xlsx Excel File  
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                else
                {
                    var list = new List<SupplierOrder>(ItemsInserted);

                    var item = new SupplierOrder();

                    list.Add(item);

                    return View("CreateOrder", list);
                }
            }
        }

        [HttpPost]
        public ActionResult CreateListingLinkItems(List<SellerItemLink> ItemsInserted)
        {
            if (!ModelState.IsValid)
            {
                Attention("Error Creating Listing - Please make sure that all details are entered");

                ViewBag.SellerList = _listingProvider.GetSellerList();

                return View("CreateListingLink", ItemsInserted);
            }

            foreach (var link in ItemsInserted)
            {
                _listingProvider.UpdateSellerItemLink(link);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateListingItems(List<BulkInsert> ItemsInserted)
        {
            if (!ModelState.IsValid)
            {
                Attention("Error Creating Listing - Please make sure that all details are entered");

                ViewBag.EbayCategory = _itemProvider.GetEbayCategories();
                ViewBag.AmazonCategory = _itemProvider.GetAmazonCategories();
                ViewBag.ListingStatus = _listingProvider.GetListingStatus();
                ViewBag.ListingChannelID = _listingProvider.GetListingChannels();


                return View("CreateListings", ItemsInserted);
            }

            _listingProvider.CreateListingItems(ItemsInserted);

            Information("Listing requests created successfully|");

            return RedirectToAction("Index");

        }

        public JsonResult AddItemCategory(ItemCategory category)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsAddedSuccess = _itemProvider.AddItemCategory(category);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        public JsonResult AddAmazonItemCategory(AmazonCategory category)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsAddedSuccess = _itemProvider.AddAmazonItemCategory(category);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        public JsonResult AddEbayItemCategory(EbayCategory category)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsAddedSuccess = _itemProvider.AddEbayItemCategory(category);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        [HttpPost]
        public JsonResult UpdateItemCategory(ItemCategory category)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsUpdatedSuccess = _itemProvider.UpdateItemCategory(category);

            if (IsUpdatedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        [HttpPost]
        public JsonResult UpdateEbayItemCategory(EbayCategory category)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsUpdatedSuccess = _itemProvider.UpdateEbayItemCategory(category);

            if (IsUpdatedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        [HttpPost]
        public JsonResult UpdateAmazonItemCategory(AmazonCategory category)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsUpdatedSuccess = _itemProvider.UpdateAmazonItemCategory(category);

            if (IsUpdatedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }


        public JsonResult AddItemLocation(ItemLocation location)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsAddedSuccess = _itemProvider.AddItemLocation(location);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }


        public ActionResult GetOrderRequestForm(int id) //---------------------------try------------
        {
            var items = _itemProvider.GetItemsBySupplier(id);
            ViewBag.SupplierID = id;
            return PartialView("_SupplierOrderRequest", items);
        }

        [HttpPost]
        [WordDocument]
        public ActionResult GenerateOrderRequestForm(OrderRequest ordersRequested)
        {
            ordersRequested.SupplierRef = new SupplierProvider(new SupplierRepository()).GetSupplierByID(ordersRequested.SupplierID);

            foreach (var item in ordersRequested.ItemsRequested.Where(r => r.RequestedItemQuantity > 0))
            {
                item.ItemRef = _itemProvider.GetItemByID(item.ItemMasterID);
            }

            ViewBag.WordDocumentFilename = ordersRequested.SupplierRef.SupplierName + "-" + DateTime.Now.ToShortDateString();

            //return new PdfResult(ordersRequested, "SupplierOrderForm");

            return View("SupplierOrderForm", ordersRequested);

        }

        public ActionResult GetItemQuantityRequestForm(int id)
        {
            var items = _itemProvider.GetItemsBySupplier(id);
            ViewBag.SupplierID = id;
            return PartialView("_UpdateStock", items);
        }

        [HttpPost]
        public ActionResult UpdateItemQuantity(OrderRequest ordersRequested)
        {
            bool updateStatus = _itemProvider.UpdateItemStock(ordersRequested);

            loadSupplierList(ordersRequested.SupplierID);


            return View("Stock");

        }

        [HttpPost]
        public JsonResult UpdateItemLocation(ItemLocation location)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsUpdatedSuccess = _itemProvider.UpdateItemLocation(location);

            if (IsUpdatedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }




        [HttpPost]
        public JsonResult AddItemColor(string Color, int ItemColorID)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            ItemColor color = new ItemColor() { Color = Color, ItemColorID = ItemColorID };

            bool IsAddedSuccess = _itemProvider.AddItemColor(color);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }


        [HttpPost]
        public JsonResult UpdateItemColor(string Color, int ItemColorID)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            ItemColor color = new ItemColor() { Color = Color, ItemColorID = ItemColorID };

            bool IsUpdatedSuccess = _itemProvider.UpdateItemColor(color);

            if (IsUpdatedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        [HttpPost]
        public JsonResult AddCarrier(PostalCarrier carrier)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            //PostalCarrier carrier = new PostalCarrier() { PostalCarrierName = PostalCarrierName, PostalCarrierID = PostalCarrierID };

            bool IsAddedSuccess = _carrierProvider.AddCarrier(carrier);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }


        [HttpPost]
        public JsonResult UpdateCarrier(PostalCarrier carrier)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            //PostalCarrier carrier = new PostalCarrier() { PostalCarrierName = PostalCarrierName, PostalCarrierID = PostalCarrierID };

            bool IsAddedSuccess = _carrierProvider.UpdateCarrier(carrier);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }


        [HttpPost]
        public JsonResult AddItemManufacturer(ItemManufacturer manufacturer)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsAddedSuccess = _itemProvider.AddItemManufacturer(manufacturer);

            if (IsAddedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }


        [HttpPost]
        public JsonResult UpdateItemManufacturer(ItemManufacturer manufacturer)
        {
            if (!ModelState.IsValid)
            {
                return Json(MessageConstants.UNEXPECTED_ERROR);
            }

            bool IsUpdatedSuccess = _itemProvider.UpdateItemManufacturer(manufacturer);

            if (IsUpdatedSuccess)
                return Json(MessageConstants.MESSAGE_SUCCESS);
            else
                return Json(MessageConstants.UNEXPECTED_ERROR);
        }

        public void loadStockSource()
        {
            var source = _itemProvider.GetStockSource();

            ViewBag.InSource = source.Purchase;
            ViewBag.OutSource = source.Sale;
        }


        private void loadInventoryLocation(object selectedLocation = null)
        {
            var InvLocation = _itemProvider.InventoryLocation();

            ViewBag.InvLocation = new SelectList(InvLocation, "FBARootID", "FBARoot", selectedLocation);
        }
        private void loadSupplierList(object selectedSupplier = null)
        {
            var suppliers = _itemProvider.GetSuppliers();

            ViewBag.SupplierID = new SelectList(suppliers, "SupplierID", "SupplierName", selectedSupplier);
        }

        private void loadCategoryList(object selectedCategory = null)
        {
            var categories = _itemProvider.GetCategories();

            ViewBag.ItemCategoryID = new SelectList(categories, "ItemCategoryID", "CategoryName", selectedCategory);
        }


        private void loadManufacturerList(object selectedManufacturer = null)
        {
            var manufacturers = _itemProvider.GetManufacturers();

            ViewBag.ItemManufacturerID = new SelectList(manufacturers, "ItemManufacturerID", "ManufacturerName", selectedManufacturer);
        }

        private void loadColorList(object selectedColor = null)
        {
            var suppliers = _itemProvider.GetColors();

            ViewBag.ItemColorID = new SelectList(suppliers, "ItemColorID", "Color", selectedColor);
        }


        private void loadLocationList(object selectedColor = null)
        {
            var locations = _itemProvider.GetLocations();

            ViewBag.LocationID = new SelectList(locations, "LocationID", "LocationName", selectedColor);
        }


    }
}
