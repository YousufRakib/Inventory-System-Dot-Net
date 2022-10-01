using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models
{
    public class ItemMasterMapper : ClassMapper<ItemMaster>
    {
        public ItemMasterMapper()
        {
            Table("ItemMasters");
            Map(i => i.SupplierName).Ignore();
            Map(i => i.TotalCost).Ignore();
            Map(i => i.ManufacturerName).Ignore();
            Map(i => i.CategoryName).Ignore();
            Map(i => i.ActiveListings).Ignore();
            Map(i => i.CBM3).Ignore();
            Map(i => i.StockIn).Ignore();
            Map(i => i.StockOut).Ignore();
            Map(i => i.CurrentStock).Ignore();
            Map(i => i.SKU).Ignore();
            Map(i => i.UserID).Ignore();
            Map(i => i.ListingItemNo).Ignore();
            Map(i => i.SellerIndex).Ignore();
            Map(i => i.UKSold7Days).Ignore();
            Map(i => i.EUSold7Days).Ignore();
            Map(i => i.USASold7Days).Ignore();
            Map(i => i.UKSold30Days).Ignore();
            Map(i => i.EUSold30Days).Ignore();
            Map(i => i.USASold30Days).Ignore();
            Map(i => i.UKWarehouse).Ignore();
            Map(i => i.EUWarehouse).Ignore();
            Map(i => i.USAWarehouse).Ignore();
            Map(i => i.Dimension1).Ignore();
            Map(i => i.FBARootID).Ignore();

            Map(i => i.CostUk).Ignore();
            Map(i => i.UkWhStock).Ignore();
            Map(i => i.UKFBAStock).Ignore();
            Map(i => i.NEEZUKFBAStock).Ignore();
            Map(i => i.UKTotalStock).Ignore();
            Map(i => i.UKStockLife).Ignore();
            Map(i => i.UKReOrderQTY).Ignore();
            Map(i => i.CostEU).Ignore();
            Map(i => i.EUWhStock).Ignore();
            Map(i => i.EUFBAStock).Ignore();
            Map(i => i.NEEZEUFBAStock).Ignore();
            Map(i => i.CDisFBCStock).Ignore();
            Map(i => i.EUTotalStock).Ignore();
            Map(i => i.EUStockLevel).Ignore();
            Map(i => i.EUStockLife).Ignore();
            Map(i => i.CostUSA).Ignore();
            Map(i => i.USAWhStock).Ignore();
            Map(i => i.USAFBAStock).Ignore();
            Map(i => i.NEEZUSAFBAStock).Ignore();
            Map(i => i.USATotalStock).Ignore();
            Map(i => i.USAStockLevel).Ignore();
            Map(i => i.USAStockLife).Ignore();
            Map(i => i.CostCA).Ignore();
            Map(i => i.CAWhStock).Ignore();
            Map(i => i.CAFBAStock).Ignore();
            //Map(i => i.NEEZUSAFBAStock).Ignore();
            Map(i => i.CATotalStock).Ignore();
            Map(i => i.CAStockLevel).Ignore();
            Map(i => i.CAStockLife).Ignore();
            Map(i => i.CostAU).Ignore();
            Map(i => i.AUWhStock).Ignore();
            Map(i => i.AUFBAStock).Ignore();
            //Map(i => i.NEEZUSAFBAStock).Ignore();
            Map(i => i.AUTotalStock).Ignore();
            Map(i => i.AUStockLevel).Ignore();
            Map(i => i.AUStockLife).Ignore();
            Map(i => i.NEEZCAFBAStock).Ignore();
            Map(i => i.NEEZAUFBAStock).Ignore();
            Map(i => i.AUSold7Days).Ignore();
            Map(i => i.CASold7Days).Ignore();
            Map(i => i.AUSold30Days).Ignore();
            Map(i => i.CASold30Days).Ignore();
            Map(i => i.AUWarehouse).Ignore();
            Map(i => i.CAWarehouse).Ignore();
            Map(i => i.FBAStockFNSKUUK).Ignore();
            Map(i => i.FBAStockFNSKUEU).Ignore();
            Map(i => i.FBAStockFNSKUUS).Ignore();
            Map(i => i.FBAStockFNSKUCA).Ignore();
            Map(i => i.FBAStockFNSKUAU).Ignore();
            Map(i => i.UKSold7DaysFNSKU).Ignore();
            Map(i => i.AUSold7DaysFNSKU).Ignore();
            Map(i => i.USASold7DaysFNSKU).Ignore();
            Map(i => i.CASold7DaysFNSKU).Ignore();
            Map(i => i.EUSold7DaysFNSKU).Ignore();

            AutoMap();
        }
    }

    public class ItemStockMapper : ClassMapper<ItemStock>
    {
        public ItemStockMapper()
        {
            Table("ItemStock");
            AutoMap();
        }
    }

    public class StockViewMapper : ClassMapper<StockView>
    {
        public StockViewMapper()
        {
            Table("StockView");
            Map(i => i.IsUpdated).Ignore();
            AutoMap();
        }
    }

    public class StockView
    {
        public int ItemMasterID { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int CurrentStock { get; set; }
        public bool IsUpdated { get; set; }
        //Kamrul
        //public bool IsThisFBAStock { }

        ///comment for error in sunchronize by kamrul
        //public int WearHouseId { get; set; }
        //public int FBARootId { get; set; }
    }
    public class StockViewForStockManagement
    {
        public int ItemMasterID { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int CurrentStock { get; set; }
        public bool IsUpdated { get; set; }
        //Kamrul
        //public bool IsThisFBAStock { }
        public int WearHouseID { get; set; }
        public int FBARootID { get; set; }
    }

    public class ItemStockHistory
    {
        public int StockIn { get; set; }

        public string SupplierName { get; set; }

        public string Source { get; set; }

        public int StockOut { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedDateString
        {
            get
            {
                return string.Format("{0:dd/MM/yyyy}", this.CreatedDate);
            }
        }

        public string Notes;
        public string ItemCost { get; set; }
        public string FBARootName { get; set; }

        public string FNSKU { get; set; }

    }

    public class ItemStock
    {
        public int StockId { get; set; }
        // public string ItemName { get; set; }

        public int ItemMasterID { get; set; }

        public int StockIn { get; set; }

        public int StockOut { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }

        public int InSource { get; set; }

        public int OutSource { get; set; }

        public string ListingItemNo { get; set; }

        public string SKU { get; set; }

        public string OrderReferenceNo { get; set; }

        public int SellerIndex { get; set; }

        public string SellerItemName { get; set; }

        public int FBARootID { get; set; } // Inventory Location 18-02-2021

        public string BatchNo { get; set; }
        public string FnSku { get; set; }

        public double Vat { get; set; }
        public double OriginalPrice { get; set; }
        public double Price { get; set; }

        public int Type { get; set; }
    }

    public class ItemDetails
    { //For Item Load List by Seller 

        //-----------For Get Method------------//
        public int StockId { get; set; }
        public int FBARequestID { get; set; }
        public string ItemName { get; set; }
        public string Dimension { get; set; }
        public string ItemWeight { get; set; }
        public double CartonQty { get; set; }
        public string ItemCode { get; set; }
        public double ItemCost { get; set; }
        public int ItemMasterID { get; set; }
        public double MasterCartonQty { get; set; }
        public int UserID { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public string ListingItemNo { get; set; }
        public string SKU { get; set; }
        public string FNSKU { get; set; }
        public string OrderReferenceNo { get; set; }
        public int SellerIndex { get; set; }
        public string SellerItemName { get; set; }
        public int FBARootID { get; set; }// Inventory Location 18-02-2021

        public double UKSold7Days { get; set; }
        public double EUSold7Days { get; set; }
        public double USASold7Days { get; set; }

        public double UKSold30Days { get; set; }
        public double EUSold30Days { get; set; }
        public double USASold30Days { get; set; }
        public double UKWarehouse { get; set; }
        public double EUWarehouse { get; set; }
        public double USAWarehouse { get; set; }
        public double UKFbaStock { get; set; }



    }

    public class FBALocations
    {
        public int FBARootID { get; set; }
        public string FBARoot { get; set; }

    }
    public class ItemList
    {
        public int ItemMasterID { get; set; }
        public string ItemName { get; set; }

    }

    public class StockSource
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class StockSourceList
    {
        public List<StockSource> Purchase { get; set; }
        public List<StockSource> Sale { get; set; }
    }

    public class StockOutMapper : ClassMapper<StockOut>
    {
        public StockOutMapper()
        {
            Table("StockOut");
            AutoMap();
        }
    }

    public class StockOut
    {
        public int Id { get; set; }

        public string Source { get; set; }
    }

    public class ItemMaster
    {
        public int ItemMasterID { get; set; }//

        [Required]
        public string ItemName { get; set; }//

        [Required]
        public string ItemCode { get; set; }//

        [Required]
        public string Description { get; set; }//

        [Required]
        public string Brand { get; set; }//

        [Required]
        public string Dimension { get; set; }//

        [Required]
        public string ItemWeight { get; set; }//

        [Required]
        [Range(1, 100, ErrorMessage = "VAT should be in the range of 1-100")]
        public int VAT { get; set; }//

        [Required]
        public double ItemCost { get; set; }//

        [Required]
        public double SellingPrice { get; set; }//

        [Required]
        [Display(Name = "Length")]
        public double Length { get; set; }//

        [Required]
        [Display(Name = "Width")]
        public double Width { get; set; }//

        [Required]
        [Display(Name = "Height")]
        public double Height { get; set; }//

        public double TotalCost { get; set; }

        [Display(Name = "Lead Time")]
        [Range(1, 365)]
        public int LeadTime { get; set; }//

        [Display(Name = "CBM3")]
        public double CBM3
        {
            get
            {
                return Math.Round(Length * Width * Height / 1000000 * 10000) / 10000;
            }
        }


        [Required]
        public string BarCode { get; set; }//

        [Required]
        public string FNSKU { get; set; }//

        public string Notes { get; set; }//

        [Display(Name = "Supplier")]
        [Required]
        public int SupplierID { get; set; }//

        public string SupplierName { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int ItemCategoryID { get; set; }//

        [Display(Name = "Manufacturer")]
        [Required]
        public int ItemManufacturerID { get; set; }//

        [Display(Name = "Color")]
        [Required]
        public int ItemColorID { get; set; }//

        [Display(Name = "Location")]
        [Required]
        public int LocationID { get; set; }//

        [Required]
        [Display(Name = "QTY in Master Carton")]
        public double MasterCartonQty { get; set; }//

        [Display(Name = "Stock Reorder Level")]
        [Required]
        public int ReOrderLevel { get; set; }//

        [Display(Name = "Submission Date")]
        [Required]
        public DateTime? DateOfSubmission { get; set; }//

        public DateTime? CreatedDate { get; set; }//

        public DateTime? ModifiedDate
        {
            get
            {
                return DateTime.Now;
            }
        }//
        [Display(Name = "Carton Qty")]
        [Required]
        public decimal CartonQty
        {
            get; set;
        }//

        public bool IsActive { get; set; }//

        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public int ActiveListings { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int StockUnits { get; set; }//
        public int CurrentStock { get; set; }

        //____________________DISPLAY ITEMS____________________//
        public string CostUk { get; set; }
        public string UkWhStock { get; set; }
        public string UKFBAStock { get; set; }
        public string NEEZUKFBAStock { get; set; }
        public string UKTotalStock { get; set; }
        public string UKStockLife { get; set; }
        public string UKReOrderQTY { get; set; }

        public string CostEU { get; set; }
        public string EUWhStock { get; set; }
        public string EUFBAStock { get; set; }
        public string NEEZEUFBAStock { get; set; }
        public string CDisFBCStock { get; set; }
        public string EUTotalStock { get; set; }
        public string EUStockLevel { get; set; }
        public string EUStockLife { get; set; }

        public string CostUSA { get; set; }
        public string USAWhStock { get; set; }
        public string USAFBAStock { get; set; }
        public string NEEZUSAFBAStock { get; set; }
        public string USATotalStock { get; set; }
        public string USAStockLevel { get; set; }
        public string USAStockLife { get; set; }

        public string CostCA { get; set; }
        public string CAWhStock { get; set; }
        public string CAFBAStock { get; set; }
        public string NEEZCAFBAStock { get; set; }//
        public string CATotalStock { get; set; }
        public string CAStockLevel { get; set; }
        public string CAStockLife { get; set; }

        public string CostAU { get; set; }
        public string AUWhStock { get; set; }
        public string AUFBAStock { get; set; }
        public string NEEZAUFBAStock { get; set; }//
        public string AUTotalStock { get; set; }
        public string AUStockLevel { get; set; }
        public string AUStockLife { get; set; }

        public string SKU { get; set; }
        public string ListingItemNo { get; set; }
        public int SellerIndex { get; set; }
        public int UserID { get; set; }

        public double UKSold7Days { get; set; }
        public double EUSold7Days { get; set; }
        public double USASold7Days { get; set; }
        public double AUSold7Days { get; set; }//
        public double CASold7Days { get; set; }//

        public double UKSold30Days { get; set; }
        public double EUSold30Days { get; set; }
        public double USASold30Days { get; set; }
        public double AUSold30Days { get; set; }//
        public double CASold30Days { get; set; }//

        public double UKWarehouse { get; set; }
        public double EUWarehouse { get; set; }
        public double USAWarehouse { get; set; }
        public double AUWarehouse { get; set; }//
        public double CAWarehouse { get; set; }//

        public double UKFbaStock { get; set; }
        public double EUFbaStock { get; set; }
        public double USAFbaStock { get; set; }
        public double CAFbaStock { get; set; }
        public double AUFbaStock { get; set; }


        public double FBAStockFNSKUUK { get; set; }
        public double FBAStockFNSKUEU { get; set; }
        public double FBAStockFNSKUUS { get; set; }
        public double FBAStockFNSKUCA { get; set; }
        public double FBAStockFNSKUAU { get; set; }

        public double UKSold7DaysFNSKU { get; set; }
        public double AUSold7DaysFNSKU { get; set; }
        public double USASold7DaysFNSKU { get; set; }
        public double CASold7DaysFNSKU { get; set; }
        public double EUSold7DaysFNSKU { get; set; }

        //public double AfnTotalQuantity { get; set; }


        public string Dimension1 { get; set; }

        public int FBARootID { get; set; } //ItemStock

    }

    public class ItemMasterViewModel
    {
        public int ItemMasterID { get; set; }//

        [Required]
        public string ItemName { get; set; }//

        [Required]
        public string ItemCode { get; set; }//

        [Required]
        public string Description { get; set; }//

        [Required]
        public string Brand { get; set; }//

        [Required]
        public string Dimension { get; set; }//

        [Required]
        public string ItemWeight { get; set; }//

        [Required]
        [Range(1, 100, ErrorMessage = "VAT should be in the range of 1-100")]
        public int VAT { get; set; }//

        [Required]
        public double ItemCost { get; set; }//

        [Required]
        public double SellingPrice { get; set; }//

        [Required]
        [Display(Name = "Length")]
        public double Length { get; set; }//

        [Required]
        [Display(Name = "Width")]
        public double Width { get; set; }//

        [Required]
        [Display(Name = "Height")]
        public double Height { get; set; }//

        [Display(Name = "Lead Time")]
        [Range(1, 365)]
        public int LeadTime { get; set; }//

        [Display(Name = "CBM3")]
        public double CBM3
        {
            get
            {
                return Math.Round(Length * Width * Height / 1000000 * 10000) / 10000;
            }
        }


        [Required]
        public string BarCode { get; set; }//

        [Required]
        public string FNSKU { get; set; }//

        public string Notes { get; set; }//

        [Display(Name = "Supplier")]
        [Required]
        public int SupplierID { get; set; }//

        [Display(Name = "Category")]
        [Required]
        public int ItemCategoryID { get; set; }//

        [Display(Name = "Manufacturer")]
        [Required]
        public int ItemManufacturerID { get; set; }//

        [Display(Name = "Color")]
        [Required]
        public int ItemColorID { get; set; }//

        [Display(Name = "Location")]
        [Required]
        public int LocationID { get; set; }//

        [Required]
        [Display(Name = "QTY in MC")]
        public double MasterCartonQty { get; set; }//

        [Display(Name = "Stock Reorder Level")]
        [Required]
        public int ReOrderLevel { get; set; }//

        [Display(Name = "Submission Date")]
        [Required]
        public DateTime? DateOfSubmission { get; set; }//

        public DateTime? CreatedDate { get; set; }//

        public DateTime? ModifiedDate
        {
            get
            {
                return DateTime.Now;
            }
        }//
        [Display(Name = "Carton/Box Weight(KG)")]
        [Required]
        public decimal CartonQty
        {
            get; set;
        }//

        public bool IsActive { get; set; }//

        public int StockUnits { get; set; }//
        
        public string NEEZCAFBAStock { get; set; }//
        
        public string NEEZAUFBAStock { get; set; }//
        
    }


    public class UserInformation
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }

    }

    public class InventoryLocation
    {
        public string FBARootID { get; set; }
        public string FBARoot { get; set; }
        public string Del { get; set; }
    }
}
