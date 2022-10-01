using AkraTechFramework.Controllers;
using Infrastructure.Core.Provider;
using Infrastructure.Core.Provider.Interfaces;
using Infrastrucutre.Core.DataAccess;
using Infrastrucutre.Core.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManager.Controllers
{
    [Authorize]
    public class ItemPriceController : BaseController
    {

        IItemProvider _itemProvider;
        IItemPriceProvider _ItemPriceProvider;
        public ItemPriceController(IItemPriceProvider ItemPriceProvider, IItemProvider itemProvider)
        {
            this._ItemPriceProvider = ItemPriceProvider;
            this._itemProvider = itemProvider;
        }

        public ItemPriceController()
            : this(new ItemPriceProvider(new ItemPriceRepository()),new ItemProvider(new ItemRepository()))
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult RemoveItemPrice(int id) //Update Fba Pending 
        {
            if (IsAdmin)
            {
                if (_ItemPriceProvider.RemoveItemPrice(id))
                {
                    return Json(new { Result = "OK", Message = "Data remove successfully!!" });
                }
            }
            else if (IsAlraze)
            {
                if (_ItemPriceProvider.RemoveItemPrice(id))
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

        public JsonResult GetItemPrices()
        {
            var items = _ItemPriceProvider.GetItemPrices();
            return Json(new { Result = "OK", Records = items });
        }

        public JsonResult GetItemPriceHistory(int id, int jtStartIndex = 0, int jtPageSize = 0)
        {
            int rowCount = 0;
            var items = _ItemPriceProvider.GetItemPricesHistory(id, out rowCount, jtStartIndex, jtPageSize);
            return Json(new { Result = "OK", Records = items, TotalRecordCount = rowCount, CurrentPage = jtStartIndex });
        }

        [HttpGet]
        public JsonResult LookupItem(string searchText, bool onlyItemName = false)
        {
            var items = _itemProvider.GetItemNames(searchText, onlyItemName);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemPrice()
        {
            loadInventoryLocation();
            loadItemList();
            ViewBag.Mode = "Create";
            ViewBag.VatVal = 20;
            return View();
        }


        public ActionResult GetItemPriceByID(int id)
        {
            ItemPrice item = _ItemPriceProvider.GetItemPriceByID(id);
             
                ViewBag.VatVal = item.Vat;
             
            loadInventoryLocation(item.FBARootId);
            loadItemList(item.ItemMasterID);
            return View("AddItemPrice", item);
        }

        [HttpPost]
        public ActionResult UpdateItemPrice(ItemPrice item)
        {
            bool IsUpdated = false;
            if (ModelState.IsValid)
            {
                IsUpdated = _ItemPriceProvider.UpdateItemPrice(item);
            }

            if (IsUpdated)
            {
                Success("ItemPrice Created Successfully.");
                return RedirectToAction("Index");
            }
            else
            {
                Attention("Please verify the fields you have entered");
                return View(item);
            }
        }

        //public JsonResult SaveItemPrice(string fdbRootId, string itemMasterID, string vat, string originalPrice, string itemPriceID)
        //{
        //    bool IsUpdated = false;
        //    ItemPrice itemPrice = new ItemPrice();
        //    itemPrice.IsItActive = true;
        //    itemPrice.FBARootId =Convert.ToInt32(fdbRootId);
        //    itemPrice.ItemMasterID = Convert.ToInt32(itemMasterID);
        //    itemPrice.Vat = Convert.ToDouble(vat);
        //    itemPrice.OriginalPrice = Convert.ToDouble(originalPrice);
        //    itemPrice.ItemPriceID = Convert.ToInt32(itemPriceID);

        //    IsUpdated = _ItemPriceProvider.AddItemPrice(itemPrice);

        //    if (IsUpdated)
        //    {
        //        return Json(new { Result = "OK" });
        //    }
        //    else
        //    {
        //        return Json(new { Result = "ERROR", Message = "UnExpected Error" });
        //    }
        //}

        [HttpPost]
        public ActionResult AddItemPrice(ItemPrice item, string command)
        {
            bool IsUpdated = false;
            item.IsItActive = true;

            if (ModelState.IsValid)
            {
                if (command == "Save")
                {
                    IsUpdated = _ItemPriceProvider.AddItemPrice(item);
                }
                else if (command == "Update")
                {
                    IsUpdated = _ItemPriceProvider.UpdateItemPrice(item);
                }
            }

            if (IsUpdated)
            {
                if (command == "Save")
                {
                    Success("ItemPrice Created Successfully.");
                }
                else
                {
                    Success("ItemPrice Updated Successfully.");
                }

                return Redirect("Index");
            }
            else
            {
                Attention("Please verify the fields you have entered");
                return View(item);
            }

        }
        private void loadInventoryLocation(object selectedLocation = null)
        {
            var InvLocation = _itemProvider.InventoryLocation();

            ViewBag.InvLocation = new SelectList(InvLocation, "FBARootID", "FBARoot", selectedLocation);
        }

        private void loadItemList(object selectedLocation = null)
        {
            List<ItemList> lstItemMaster = _itemProvider.GetItemMasterList().Select(x=>new ItemList() { 
            ItemMasterID = x.ItemMasterID,
            ItemName = x.ItemName
            }).ToList();
            ViewBag.ItemList = new SelectList(lstItemMaster, "ItemMasterId", "ItemName", selectedLocation);
        }
    }
}
