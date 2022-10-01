using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eBay.Service.Core.Soap;
using eBay.Service.Core.Sdk;
using eBay.Service.Call;
using eBay.Service.Util;
using System.Configuration;
using AkraTechFramework.Controllers;
using Infrastrucutre.Core.Models;
using Infrastructure.Core.Provider;
using Infrastrucutre.Core.DataAccess;
using InventoryManager.CustomFilters;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Constants;
using System.Threading;
using RestSharp;
using MWSClientCsRuntime;
using System.IO;
using System.Xml;
using Inventory.Synchronizer;
using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.Ajax.Utilities;
using Infrastructure.Core.Provider.Interfaces;
using Infrastrucutre.Core.Models.ViewModels;
using Newtonsoft.Json;
using AT.Core.Common;
using Infrastrucutre.Core.Models.Models;
using Syncfusion.XlsIO;

namespace InventoryManager.Controllers
{
    [Authorize]
    public class ForecastingController : BaseController
    {
        IItemProvider _itemProvider;
        IForecastingProvider _forecastingProvider;
        IListingProvider _listingProvider;

        public ForecastingController(IItemProvider itemProvider, IForecastingProvider forecastingProvider, IListingProvider listingProvider)
        {
            this._itemProvider = itemProvider;
            this._forecastingProvider = forecastingProvider;
            this._listingProvider = listingProvider;

        }

        public ForecastingController() : this(new ItemProvider(new ItemRepository()), new ForecastingProvider(new ForecastingRepository()), new ListingProvider(new ListingRepository()))
        {
        }

        [HttpGet]
        public ActionResult ForecastingView()
        {
            return View();
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

        public JsonResult GetForecastingView(string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string forecastingPeriod, string increment, string depositDays, string manufacturePeriodAndShipingDays, string filterText = "")
        {
            if (currentStartDate == "" && currentEndDate == "" && lastYearStartDate == "" && lastYearEndDate == "" && forecastingPeriod == "" && increment == "" && depositDays == "" && manufacturePeriodAndShipingDays == "" && filterText == "")
            {
                return Json(new { Result = "OK", Records = new List<ForecastingViewModel>() });
            }
            var items = _forecastingProvider.GetForecastingViewData(currentStartDate, currentEndDate, lastYearStartDate, lastYearEndDate, forecastingPeriod, increment, depositDays, manufacturePeriodAndShipingDays, filterText);

            return Json(new { Result = "OK", Records = items });
        }

        public ActionResult ItemForecasting()
        {
            ViewBag.UserName = User.Identity.ToGenericUserIdentity().Name;
            ViewBag.OrderId = _forecastingProvider.GetOrderId();


            loadItem();
            loadUserList();
            loadFbaRootList();
            return View();
        }

        private void loadUserList(object selectedItem = null)
        {
            var UserID = _listingProvider.GetUser();
            ViewBag.UserID = new SelectList(UserID, "UserID", "UserName", selectedItem);
        }

        public void loadFbaRootList(object selectedItem = null) //FBA Root List
        {
            var FBARoot = _listingProvider.GetFBARootList();
            ViewBag.FBARoot = new SelectList(FBARoot, "FBARootID", "FBARoot", selectedItem);
        }

        public void loadItem(object selectedItem = null)
        {
            var items = _itemProvider.GetItemNames();
            var itemList = items.Select(x => new
            {
                ItemMasterID = x.ItemMasterID,
                ItemName = x.ItemName
            }).ToList();
            itemList.Insert(0, new { ItemMasterID = 0, ItemName = "SELECT" });

            ViewBag.Items = new SelectList(items, "ItemMasterID", "ItemName", selectedItem);
        }

        public JsonResult GetsItemStock(int fbaRootId, int itemMasterId)
        {
            ItemStockViewModel itemStock = _forecastingProvider.GetItemsStock(fbaRootId, itemMasterId);
            return Json(itemStock);
        }

        public JsonResult GetsItemForecastingInfo(int fbaRootId, int itemMasterId, string currentStartDate, string currentEndDate, string lastYearStartDate, string lastYearEndDate, string incrementTarget, string averageDurationDays)
        {
            ItemStockViewModel itemStock = _forecastingProvider.GetsItemForecastingInfo(fbaRootId, itemMasterId, currentStartDate, currentEndDate, lastYearStartDate, lastYearEndDate, incrementTarget, averageDurationDays);
            return Json(itemStock);
        }

        public JsonResult GetsItemForecastingInfo2(int? depositPeriod, int? shippingDays, decimal? currentStockSurvivalDays, int? averageDurationDays, decimal? incrementSold, int? extendPeriod)
        {
            ItemStockViewModel itemStock = new ItemStockViewModel();
            itemStock.ExtraStock =Convert.ToDecimal(currentStockSurvivalDays - (depositPeriod + shippingDays));
            itemStock.OrderReceivingDaysBeforeSelling = Convert.ToDecimal(depositPeriod + shippingDays);
             
            if(currentStockSurvivalDays < itemStock.OrderReceivingDaysBeforeSelling)
            {
                itemStock.ActualForecastingDays =Convert.ToDecimal(averageDurationDays);
            }
            else
            {
                itemStock.ActualForecastingDays =Convert.ToDecimal(averageDurationDays - itemStock.ExtraStock);
            }

            itemStock.TotalQtyNeedToOrder = Convert.ToDecimal(itemStock.ActualForecastingDays * incrementSold);

            if (incrementSold != 0)
            {
                itemStock.LessPeriod = Convert.ToDecimal(itemStock.TotalQtyNeedToOrder / incrementSold);
            }
            else
            {
                itemStock.LessPeriod = 0;
            }
            //itemStock.LessPeriod= Convert.ToDecimal(itemStock.TotalQtyNeedToOrder/ incrementSold);

            if (itemStock.TotalQtyNeedToOrder < 1)
            {
                itemStock.OrderStatus = "Over Stock";
            }
            else
            {
                itemStock.OrderStatus = "Need to Order";
            }

            if (itemStock.TotalQtyNeedToOrder < 0)
            {
                itemStock.StockDaysStatus = "Days Extra Stock";
            }
            else
            {
                itemStock.StockDaysStatus = "Days Less Stock";
            }

            itemStock.TotalForecastingDays = Convert.ToDecimal(itemStock.ActualForecastingDays + extendPeriod);
            itemStock.TotalQtyNeed = Convert.ToDecimal(itemStock.TotalForecastingDays * incrementSold);

            return Json(itemStock);
        }

        public ActionResult GetsItemForecastingListData(int? itemMasterId, decimal? currentYearSold, string currentStartDate, string currentEndDate, int? warehouseStockId, decimal? totalQtyNeedToOrder)
        {
            List<ItemStockViewModel> itemStockList = _forecastingProvider.GetsItemForecastingListData(itemMasterId, currentYearSold,currentStartDate,currentEndDate, warehouseStockId, totalQtyNeedToOrder);
            
            var jsonResult = Json(new Confirmation { output = "success", msg = "Records found", returnvalue = itemStockList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult SaveItemForecasting()
        {
            try
            {
                var forecastingModel = System.Web.HttpContext.Current.Request.Params["ForecastingDataModel"];
                var forecastingDataModel = JsonConvert.DeserializeObject<ForecastingItemViewModel>(forecastingModel);

                string result = _forecastingProvider.SaveItemForecasting(forecastingDataModel);

                if (result == "1")
                {
                    return Json(new Confirmation { output = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Confirmation { output = "error"}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ItemForecastingList()
        {
            return View();
        }

        public JsonResult GetItemForecastingSummaryList()
        {
            var result = _forecastingProvider.GetForecastingItemsSummaryList();
            return Json(new { Result = "OK", Records = result });
        }

        public JsonResult RemoveItemForecasting(int id) 
        {
            //if (IsAdmin)
            //{
            //    if (_forecastingProvider.RemoveForecastingItem(id))
            //    {
            //        return Json(new { Result = "OK", Message = "Data remove successfully!!" });
            //    }
            //}
            //else if (IsAlraze)
            //{
            //    if (_forecastingProvider.RemoveForecastingItem(id))
            //    {
            //        return Json(new { Result = "OK", Message = "Data remove successfully!!" });
            //    }
            //}
            //else
            //{
            //    return Json(new { Result = "ERROR", Message = "You Are Not Admin!!" });
            //}
            //return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });

            try
            {
                if (_forecastingProvider.RemoveForecastingItem(id)==true)
                {
                    return Json(new { Result = "OK", Message = "Data remove successfully!!" });
                }
                else
                {
                    return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
                }
            }
            catch(Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
            }
        }

        public ActionResult EditForecasting(int id)
        {
            ViewBag.UserName = User.Identity.ToGenericUserIdentity().Name;
            ViewBag.Id = id;

            ForecastingItemSummary result= _forecastingProvider.GetItemForecastingByID(id);

            return View(result);
        }

        public ActionResult GetItemForecastingListDataById(int? id)
        {
            var result = _forecastingProvider.GetsItemForecastingListDataById(id);

            var jsonResult = Json(new Confirmation { output = "success", msg = "Records found", returnvalue = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult UpdateItemForecasting()
        {
            try
            {
                var forecastingModel = System.Web.HttpContext.Current.Request.Params["ForecastingDataModel"];
                var forecastingDataModel = JsonConvert.DeserializeObject<ForecastingItemViewModel>(forecastingModel);

                string result = _forecastingProvider.UpdateItemForecasting(forecastingDataModel);

                if (result == "1")
                {
                    return Json(new Confirmation { output = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SupplierOrderSelection()
        {
            ViewBag.UserName = User.Identity.ToGenericUserIdentity().Name;
            ViewBag.SupplierOrderId = _forecastingProvider.GetSupplierOrderId();

            LoadSupplierName();
            loadFbaRootList();
            return View();
        }

        public JsonResult GetsItemForecastingSummaryListForSupplierOrder(string warehouseRoot = "",string supplierName="", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            int rowCount = 0;
            var result = _forecastingProvider.GetsItemForecastingSummaryListForSupplierOrder(out rowCount, warehouseRoot,supplierName, jtStartIndex, jtPageSize);

            return Json(new { Result = "OK", Records = result, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public void LoadSupplierName(object selectedSupplier = null)
        {
            var suppliers = _forecastingProvider.GetSupplierNameFromForecasting();
            var supplierList = suppliers.Select(x => new
            {
                SupplierId=0,
                SupplierName = x.SupplierName
            }).ToList();
            supplierList.Insert(0, new { SupplierId=0, SupplierName = "SELECT" });

            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "SupplierName", selectedSupplier);
        }

        [HttpPost]
        public ActionResult SaveSupplierOrder()
        {
            try
            {
                var supplierOrderModel = System.Web.HttpContext.Current.Request.Params["SupplierOrderSelectionDataModel"];
                var supplierOrderDataModel = JsonConvert.DeserializeObject<SupplierOrderViewModel>(supplierOrderModel);

                string result = _forecastingProvider.SaveSupplierOrder(supplierOrderDataModel);

                if (result == "1")
                {
                    return Json(new Confirmation { output = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SupplierOrderList()
        {
            return View();
        }

        public JsonResult GetSupplierOrderList()
        {
            var result = _forecastingProvider.GetSupplierOrderList();
            return Json(new { Result = "OK", Records = result });
        }

        public JsonResult GetSupplierOrderListWithWarehouseRoot(string warehouseRoot = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            int rowCount = 0;
            var result = _forecastingProvider.GetSupplierOrderListWithWarehouse(out rowCount, warehouseRoot, jtStartIndex, jtPageSize);

            return Json(new { Result = "OK", Records = result, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        public JsonResult RemoveSupplierOrder(int id)
        {
            try
            {
                if (_forecastingProvider.RemoveSupplierOrder(id) == true)
                {
                    return Json(new { Result = "OK", Message = "Data remove successfully!!" });
                }
                else
                {
                    return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
            }
        }

        public ActionResult ViewSupplierOrder(int id)
        {
            ViewBag.Id = id;

            SupplierOrderSummary result = _forecastingProvider.GetSupplierOrderByID(id);

            return View(result);
        }

        public ActionResult GetSupplierOrderListDataById(int? id)
        {
            var result = _forecastingProvider.GetSupplierOrderListById(id);

            var jsonResult = Json(new Confirmation { output = "success", msg = "Records found", returnvalue = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult CreateShipment()
        {
            ViewBag.UserName = User.Identity.ToGenericUserIdentity().Name;
            ViewBag.ShipmentOrderId = _forecastingProvider.GetShipmentOrderId();
            
            loadFbaRootList();
            return View();
        }

        [HttpPost]
        public ActionResult SaveCreateShipment()
        {
            try
            {
                var createShipmentModel = System.Web.HttpContext.Current.Request.Params["CreateShipmentDataModel"];
                var createShipmentDataModel = JsonConvert.DeserializeObject<CreateShipmentViewModel>(createShipmentModel);

                createShipmentDataModel.CreateShipmentSummary.RemainingBalance = createShipmentDataModel.CreateShipmentSummary.TotalValue - createShipmentDataModel.CreateShipmentSummary.DepositAmount;
                createShipmentDataModel.CreateShipmentSummary.ETDDate = Convert.ToDateTime(createShipmentDataModel.CreateShipmentSummary.ETDDateString);
                
                string result = _forecastingProvider.SaveCreateShipment(createShipmentDataModel);

                if (result == "1")
                {
                    return Json(new Confirmation { output = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CreateShipmentList()
        {
            return View();
        }

        public JsonResult GetShipmentHistory(string id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _forecastingProvider.GetShipmentHistory(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        [HttpPost]
        public ActionResult ExportExcelFile(string id)
        {
            var fileName = "AjaxCall" + ".xlsx";

            //Save the file to server temp folder
            string fullPath = Path.Combine(Server.MapPath("~/temp"), fileName);

            //Create an instance of ExcelEngine
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2016
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;

                //Create a workbook with a worksheet
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);

                //Access first worksheet from the workbook instance
                IWorksheet worksheet = workbook.Worksheets[0];

                //Insert sample text into cell “A1”
                worksheet.Range["A1"].Text = "Hello World!";

                //Save the workbook to disk in xlsx format
                workbook.Version = ExcelVersion.Excel2016;
                workbook.SaveAs(fullPath);
            }
            var errorMessage = "you can return the errors here!";
            //Return the Excel file name
            return Json(new { fileName = fileName, errorMessage });
        }

        [HttpGet]
        public ActionResult Download(string fileName)
        {
            //Get the temp folder and file path in server
            string fullPath = Path.Combine(Server.MapPath("~/temp"), fileName);
            byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
            System.IO.File.Delete(fullPath);
            return File(fileByteArray, "application/vnd.ms-excel", fileName);
        }

        public JsonResult GetCreateShipment()
        {
            var result = _forecastingProvider.GetCreateShipmentList();
            return Json(new { Result = "OK", Records = result });
        }

        public ActionResult ViewCreateShipment(int id)
        {
            ViewBag.Id = id;

            CreateShipmentSummary result = _forecastingProvider.GetCreateShipmentByID(id);

            return View(result);
        }

        public JsonResult RemoveCreateShipment(int id)
        {
            try
            {
                if (_forecastingProvider.RemoveCreateShipment(id) == true)
                {
                    return Json(new { Result = "OK", Message = "Data remove successfully!!" });
                }
                else
                {
                    return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = "UnExpected Error!!" });
            }
        }

        public ActionResult GetCreateShipmentListDataById(int? id)
        {
            var result = _forecastingProvider.GetCreateShipmentListById(id);

            var jsonResult = Json(new Confirmation { output = "success", msg = "Records found", returnvalue = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult EditCreateShipment(int id)
        {
            ViewBag.UserName = User.Identity.ToGenericUserIdentity().Name;
            ViewBag.Id = id;

            CreateShipmentSummary result = _forecastingProvider.GetCreateShipmentByID(id);

            return View(result);
        }

        [HttpPost]
        public ActionResult UpdateCreateShipment()
        {
            try
            {
                var createShipmentModel = System.Web.HttpContext.Current.Request.Params["CreateShipmentDataModel"];
                var createShipmentDataModel = JsonConvert.DeserializeObject<CreateShipmentViewModel>(createShipmentModel);

                string result = _forecastingProvider.UpdateCreateShipment(createShipmentDataModel);

                if (result == "1")
                {
                    return Json(new Confirmation { output = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new Confirmation { output = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Costing()
        {
            return View();
        }
    }
}
