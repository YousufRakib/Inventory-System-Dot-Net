using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Mapper;

namespace Infrastrucutre.Core.Models
{
    //public class ReportMapper : ClassMapper<Report>
    //{
    //    public ReportMapper(){
    //        Table("Reports");
    //    }
    //}

    public class Report
    {
        public int ReportID { get; set; }

        public string ReportName { get; set; }

        public string ReportQuery { get; set; }

        public bool IsActive { get; set; }
    }
    //ForSmayaSalesAndSalsabilEuro
    public class SKUReportForSmayaSalesAndSalsabilEuro //SKU Report DATA 
    {
        public string ODate { get; set; }
        public string OrderDate
        {
            get
            {
                var OrDate = Convert.ToDateTime(ODate).ToString("dd/MM/yyyy");
                return OrDate.ToString();
            }
        }// OK

        public string OrderReferenceNo { get; set; } //OK
        public string MarketPlace { get; set; } // OK
        public string Country { get; set; } //OK
        public string Fulfilment { get; set; } //OK
        public int Quantity { get; set; } //OK
        public string Currency { get; set; } //OK
        public string ProductSalesPriceWithVat { get; set; } //OK
        //public Decimal AmountPaid { get; set; }
        public double ItemCost { get; set; } //OK
        public double ActualDelivery { get; set; }
        public string ActualDeliveryInString
        {
            get
            {
                return Math.Round((ActualDelivery), 2).ToString();
            }
        }
        public string FbaFees { get; set; }

        public string SellingFees
        {
            get
            {
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent =(15 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string TotalVatTax
        {
            get
            {
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (20 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string FBAHandling
        {
            get
            {
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (5 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string PromotionalCost
        {
            get
            {
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (10 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string Errors
        {
            get
            {
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (5 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string Profit //For Profit Used
        {
            get
            {
                var Profit = Math.Round(Convert.ToDouble(ProductSalesPriceWithVat) -
                    ((Convert.ToDouble(ItemCost)* Quantity) + Convert.ToDouble(FbaFees) +
                    Convert.ToDouble(SellingFees) + Convert.ToDouble(TotalVatTax) + //Convert.ToDouble(ShippingCreditsTax) +
                    Convert.ToDouble(FBAHandling) + Convert.ToDouble(PromotionalCost) +
                    Convert.ToDouble(Errors)), 2).ToString();
                //var Profit = Math.Round(Convert.ToDouble(ProductSalesPriceWithVat) -
                //    (Convert.ToDouble(ItemCost) + Convert.ToDouble(FbaFees) +
                //    Convert.ToDouble(SellingFees) + //Convert.ToDouble(VatTax) + Convert.ToDouble(ShippingCreditsTax) +
                //    Convert.ToDouble(FBAHandling) + Convert.ToDouble(PromotionalCost) +
                //    Convert.ToDouble(Errors)), 2).ToString();
                return Math.Round(Convert.ToDouble(Profit), 2).ToString();
            }
        }
        public string ProfitInGBP { get; set; }  //Not found
        public string ProfitMargin //For Profit Used
        {
            get
            {
                var ProfitMargin = Math.Round((Convert.ToDouble(Profit) / Convert.ToDouble(ItemCost) * 100), 2).ToString();
                return Math.Round(Convert.ToDouble(ProfitMargin), 2).ToString();
            }
        }
    }
    public class SKUReport //SKU Report DATA 
    {

        public int ItemMasterID { get; set; }
        public string ItemID { get; set; }
        public string SKUItemID { get; set; }
        public string OrderReferenceNo { get; set; }
        public string ItemName { get; set; }
        public string ODate { get; set; }
        public string OrderDate
        {
            get
            {
                var OrDate = Convert.ToDateTime(ODate).ToString("dd/MM/yyyy");
                return OrDate.ToString();
            }
        }
        public string SellerID { get; set; }
        public string Country { get; set; }
        public int Quantity { get; set; }
        public double ItemCost { get; set; }
        public string TotalCost { get; set; }
        public Decimal AmountPaid { get; set; }

        //kamrul
        public string Currency { get; set; }
        public string MarketPlace { get; set; }
        public string TaxCollectionModel { get; set; }
        public string Fulfilment { get; set; }

        public double ActualDelivery { get; set; }
        public string FBADeliveryFees { get; set; }
        //public string FBACharge { get; set; }
        public string SellingFees { get; set; }
        public string VatTax { get; set; }
        //public string FBAHandling { get; set; }
        //public string PromotionalCost { get; set; }
        //public string Errors { get; set; }
        //public string Profit { get; set; }
        public string ProfitInGBP { get; set; }
        //public string ProfitMargin { get; set; }

        //end kamrul


        //public double ProductSalesPriceWithVat { get; set; }
        public double ProductSalesPrice { get; set; }

        public string ProductSalesPriceWithVat
        {
            get
            {
                decimal amount = Convert.ToDecimal(ProductSalesPrice);
                decimal vatInDecimal = !string.IsNullOrEmpty(VatTax) ? Convert.ToDecimal(VatTax) : 0;
                return (amount + vatInDecimal).ToString();
            }
        }
        public double ProductSalesTax { get; set; }
        public double PostageCredits { get; set; }
        public double ShippingCreditsTax { get; set; }
        public double GiftWrapCredits { get; set; }
        public double GiftWrapCreditsTax { get; set; }
        public double PromotionalRebates { get; set; }
        public double PromotionalRebatesTax { get; set; }
        public double MarketplaceWithHeldTax { get; set; }
        //public double SellingFees { get; set; }
        public double FbaFees { get; set; }
        public double OtherTransactionFees { get; set; }
        public double Other { get; set; }


        ////public string DCharg { get; set; }
        ////public string FBACharge { get; set; }
        ////public string PVatAmount { get; set; }
        ////public string TotalCostItemCost { get; set; }
        ////public string TotalCostByQuantity { get; set; }
        ////public string PVat { get; set; }
        ////public string Promo { get; set; }
        ////public string Fees { get; set; }
        //public string Errors { get; set; }
        ////public string Addition { get; set; }
        ////public string PLevel { get; set; }
        //public string FBAHandling { get; set; }
        //public string PromotionalCost { get; set; }
        //public string Profit { get; set; }
        //public string ProfitMargin { get; set; }
        ////public string ProfitLevel { get; set; }

        public string ActualDeliveryInString
        {
            get
            {
                return Math.Round((ActualDelivery), 2).ToString();
            }
        }
        public string TotalVatTax
        {
            get
            {
                return Math.Round((Convert.ToDouble(VatTax) + ShippingCreditsTax), 2).ToString();
            }
        }
        //public string DCharg
        //{
        //    get
        //    {
        //        return Math.Round((2.44), 2).ToString();
        //    }
        //}
        //public string FBACharge
        //{
        //    get
        //    {
        //        var FBA = 2.49;
        //        var FBACharge = Math.Round(Convert.ToDouble(FBA) * (Convert.ToDouble(Quantity)), 2).ToString();
        //        return Math.Round(Convert.ToDouble(FBACharge), 2).ToString();
        //    }
        //}
        //public string PVatAmount // For Used Only PayableVat
        //{
        //    get
        //    {
        //        var per = 0.20;
        //        var Amount = Math.Round(Convert.ToDouble(AmountPaid) * Convert.ToDouble(per), 2);
        //        return Math.Round(Convert.ToDouble(Amount), 2).ToString();
        //    }
        //}
        //public string TotalCostItemCost //For Used Only PayableVat
        //{
        //    get
        //    {
        //        var Amount = Math.Round((Convert.ToDouble(TotalCost) - Convert.ToDouble(ItemCost)), 2).ToString();
        //        return Math.Round(Convert.ToDouble(Amount), 2).ToString();
        //    }
        //}
        //public string TotalCostByQuantity //TotalCost * Quantity
        //{
        //    get
        //    {
        //        var Amount = Math.Round((Convert.ToDouble(TotalCost) * Convert.ToDouble(Quantity)), 2).ToString();
        //        return Math.Round(Convert.ToDouble(Amount), 2).ToString();
        //    }
        //}
        //public string PVat
        //{ //Payable Vat
        //    get
        //    {

        //        var PVat = Math.Round((Convert.ToDouble(PVatAmount)), 2).ToString();
        //        // Math.Round((Convert.ToDouble(PVatAmount) - Convert.ToDouble(TotalCostItemCost)),2).ToString();

        //        return Math.Round(Convert.ToDouble(PVat), 2).ToString();
        //    }
        //}
        //public string Promo
        //{
        //    get
        //    {
        //        var per = 0.10;
        //        var Promo = Math.Round((Convert.ToDouble(AmountPaid)) * (Convert.ToDouble(per)), 2).ToString();
        //        return Promo;
        //    }

        //}
        //public string Fees
        //{
        //    get
        //    {
        //        var per = 0.15;
        //        var Promo = Math.Round((Convert.ToDouble(AmountPaid)) * (Convert.ToDouble(per)), 2).ToString();
        //        return Promo;
        //    }
        //}
        public string Errors
        {
            get
            {
                //var per = 0.05;
                //var Error = Math.Round((Convert.ToDouble(AmountPaid)) * (Convert.ToDouble(per)), 2).ToString();
                //return Error;
                //decimal amount = AmountPaid;
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (5 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        //public string Addition //For Profit Used
        //{
        //    get
        //    {
        //        var Addition = ((Convert.ToDouble(TotalCostByQuantity) + Convert.ToDouble(Fees) + Convert.ToDouble(Promo) + Convert.ToDouble(PVatAmount) + Convert.ToDouble(FBACharge) + Convert.ToDouble(Errors))).ToString();
        //        return Math.Round(Convert.ToDouble(Addition), 2).ToString();
        //    }
        //}

        //public string PLevel //Profit Level
        //{
        //    get
        //    {

        //        var PLevel = Math.Round((Convert.ToDouble(ProfitLevel)) * (Convert.ToDouble(Quantity))).ToString();
        //        return Math.Round(Convert.ToDouble(PLevel), 2).ToString();
        //    }
        //}
        public string FBAHandling
        {
            get
            {
                //decimal amount = AmountPaid;
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (5 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string PromotionalCost
        {
            get
            {
                //decimal amount = AmountPaid;
                decimal amount = Convert.ToDecimal(ProductSalesPriceWithVat);
                var percent = (10 * amount) / 100;
                return Math.Round(percent, 2).ToString();
            }
        }
        public string ProfitMargin //For Profit Used
        {
            get
            {
                var ProfitMargin = Math.Round((Convert.ToDouble(Profit) / Convert.ToDouble(ItemCost) * 100), 2).ToString();
                return Math.Round(Convert.ToDouble(ProfitMargin), 2).ToString();
            }
        }
        public string Profit //For Profit Used
        {
            get
            {
                //if (OrderReferenceNo == "206-6385383-7354706")
                //{

                //}
                //var Profit = Math.Round(Convert.ToDouble(AmountPaid) -
                var Profit = Math.Round(Convert.ToDouble(ProductSalesPriceWithVat) -
                    (Convert.ToDouble(ItemCost) + Convert.ToDouble(FbaFees) +
                    Convert.ToDouble(SellingFees) + Convert.ToDouble(VatTax) + Convert.ToDouble(ShippingCreditsTax) +
                    Convert.ToDouble(FBAHandling) + Convert.ToDouble(PromotionalCost) +
                    Convert.ToDouble(Errors)), 2).ToString();
                return Math.Round(Convert.ToDouble(Profit), 2).ToString();
            }
        }

        //public string ProfitLevel //Profit Level
        //{
        //    get
        //    {

        //        var PLevel = Math.Round(((Convert.ToDouble(Profit)) / (Convert.ToDouble(TotalCostByQuantity))) * 100).ToString();
        //        return Math.Round(Convert.ToDouble(PLevel), 2).ToString();
        //    }
        //}
    }

    public class Sku_Report //SKU Data Count By Country
    {
        public string ODate { get; set; }
        public string OrderDate
        {
            get
            {
                var OrDate = Convert.ToDateTime(ODate).ToString("dd/MM/yyyy");
                return OrDate.ToString();
            }
        }
        public string ItemID { get; set; }
        public string GB
        { get; set; }
        public string UnitedKingdom
        { get; set; }
        public string FR
        { get; set; }
        public string IT
        { get; set; }
        public string DE
        { get; set; }
        public string ES
        { get; set; }
        public string NL
        { get; set; }
        public string SE
        { get; set; }
        public string USA
        { get; set; }
        public string CA
        { get; set; }
        public string AU
        { get; set; }
        public string Smaya
        {
            get; set;
        }
        public string Quantity
        {
            get; set;
        }
        public string Salsabil
        { get; set; }
        public string Etsy
        { get; set; }
        public string CDisc
        { get; set; }

        public string Total
        {
            get
            {
                var Total = Math.Round(Convert.ToDouble(GB) + Convert.ToDouble(FR) + Convert.ToDouble(IT) + Convert.ToDouble(DE) + Convert.ToDouble(ES) + Convert.ToDouble(NL) + Convert.ToDouble(SE) + Convert.ToDouble(USA) + Convert.ToDouble(CA) + Convert.ToDouble(AU) + Convert.ToDouble(Smaya) + Convert.ToDouble(Salsabil) + Convert.ToDouble(Etsy) + Convert.ToDouble(CDisc)).ToString();

                return Total.ToString();
            }
        }



    }

    public class Asin_Report //Asin Data Report......
    {
        public string OrderDate { get; set; }
        public string ItemID { get; set; }
        public string GB { get; set; }
        public string DE { get; set; }
        public string FR { get; set; }
        public string IT { get; set; }
        public string ES { get; set; }
        public string NL { get; set; }
        public string PL { get; set; }
        public string SE { get; set; }
        public string TR { get; set; }
        public string USA { get; set; }
        public string CA { get; set; }
        public string AU { get; set; }
        public string Seller { get; set; }
        public string AsinNo { get; set; }
        public string FNSKU { get; set; }
        public string Salsabil { get; set; }
        public string Smays_Sales { get; set; }
        public string Etsy { get; set; }
        public string FMethod { get; set; }
        public string TotalSold
        {
            get
            {
                var TotalSold = Math.Round(Convert.ToDouble(GB) + Convert.ToDouble(FR) + Convert.ToDouble(IT) + Convert.ToDouble(DE) + Convert.ToDouble(ES) + Convert.ToDouble(NL) + Convert.ToDouble(SE) + Convert.ToDouble(USA) + Convert.ToDouble(CA) + Convert.ToDouble(Salsabil) + Convert.ToDouble(Smays_Sales)).ToString();

                return TotalSold.ToString();
            }
        }
    }

    public class AsinSub_Report //For Sub Data Report......
    {

        public string ODate
        {
            get; set;

        }
        public string OrderDate
        {
            get
            {
                var OrDate = Convert.ToDateTime(ODate).ToString("dd/MM/yyyy");
                return OrDate.ToString();
            }
        }
        public string ItemID
        { get; set; }
        public string GB
        { get; set; }
        public string FR
        { get; set; }
        public string IT
        { get; set; }
        public string DE
        { get; set; }
        public string ES
        { get; set; }
        public string NL
        { get; set; }

        public string UKNeez
        { get; set; }
        public string FRNeez
        { get; set; }
        public string ITNeez
        { get; set; }
        public string DENeez
        { get; set; }
        public string ESNeez
        { get; set; }
        public string NLNeez
        { get; set; }
        public string Samaya
        { get; set; }
        public string Salsabil
        { get; set; }

        /* public string CDisc
         { get; set; }*/
        public string Etsy
        { get; set; }

        public string TotalSold
        {
            get
            {
                var TotalQty = Math.Round(Convert.ToDouble(GB) + Convert.ToDouble(FR) + Convert.ToDouble(IT) + Convert.ToDouble(DE) + Convert.ToDouble(ES) + Convert.ToDouble(NL), 2).ToString();
                return TotalQty.ToString();
            }

        }


    }

    public class AsinSub_ReportModel
    {
        public string ODate
        {
            get; set;

        }
        public string OrderDate
        {
            get
            {
                var OrDate = Convert.ToDateTime(ODate).ToString("dd/MM/yyyy");
                return OrDate.ToString();
            }
        }
        //public string ODate { get; set; }
        public string UK_ARSUK { get; set; }
        public string DE_ARSUK { get; set; }
        public string FR_ARSUK { get; set; }
        public string IT_ARSUK { get; set; }
        public string ES_ARSUK { get; set; }
        public string NL_ARSUK { get; set; }
        public string PL_ARSUK { get; set; }
        public string SE_ARSUK { get; set; }
        public string TR_ARSUK { get; set; }
        public string USA_ARSUK { get; set; }
        public string CA_ARSUK { get; set; }
        public string AU_ARSUK { get; set; }

        public string TotalARSUK
        {
            get
            {
                var TotalQty = Math.Round(Convert.ToDouble(UK_ARSUK) +
                    Convert.ToDouble(DE_ARSUK) +
                    Convert.ToDouble(FR_ARSUK) +
                    Convert.ToDouble(IT_ARSUK) +
                    Convert.ToDouble(ES_ARSUK) +
                    Convert.ToDouble(NL_ARSUK) +
                    Convert.ToDouble(PL_ARSUK) +
                    Convert.ToDouble(SE_ARSUK) +
                    Convert.ToDouble(TR_ARSUK) +
                    Convert.ToDouble(USA_ARSUK) +
                    Convert.ToDouble(CA_ARSUK) +
                    Convert.ToDouble(AU_ARSUK), 2).ToString();
                return TotalQty.ToString();
            }
        }

        public string UK_NEEZ { get; set; }
        public string DE_NEEZ { get; set; }
        public string FR_NEEZ { get; set; }
        public string IT_NEEZ { get; set; }
        public string ES_NEEZ { get; set; }
        public string NL_NEEZ { get; set; }
        public string PL_NEEZ { get; set; }
        public string SE_NEEZ { get; set; }
        public string TR_NEEZ { get; set; }
        public string USA_NEEZ { get; set; }
        public string CA_NEEZ { get; set; }
        public string AU_NEEZ { get; set; }

        public string TotalNEEZ
        {
            get
            {
                var TotalQty = Math.Round(
                    Convert.ToDouble(UK_NEEZ) +
                    Convert.ToDouble(DE_NEEZ) +
                    Convert.ToDouble(FR_NEEZ) +
                    Convert.ToDouble(IT_NEEZ) +
                    Convert.ToDouble(ES_NEEZ) +
                    Convert.ToDouble(NL_NEEZ) +
                    Convert.ToDouble(PL_NEEZ) +
                    Convert.ToDouble(SE_NEEZ) +
                    Convert.ToDouble(TR_NEEZ) +
                    Convert.ToDouble(USA_NEEZ) +
                    Convert.ToDouble(CA_NEEZ) +
                    Convert.ToDouble(AU_NEEZ), 2).ToString();
                return TotalQty.ToString();
            }
        }

        public string Smays_Sales { get; set; }
        public string Salsabil { get; set; }
        public string Cdis { get; set; }
        public string Etsy { get; set; }
        public string Others { get; set; }
        public string TotalSold
        {
            get
            {
                var TotalQty = Math.Round(Convert.ToDouble(UK_ARSUK) +
                    Convert.ToDouble(DE_ARSUK) +
                    Convert.ToDouble(FR_ARSUK) +
                    Convert.ToDouble(IT_ARSUK) +
                    Convert.ToDouble(ES_ARSUK) +
                    Convert.ToDouble(NL_ARSUK) +
                    Convert.ToDouble(PL_ARSUK) +
                    Convert.ToDouble(SE_ARSUK) +
                    Convert.ToDouble(TR_ARSUK) +
                    Convert.ToDouble(USA_ARSUK) +
                    Convert.ToDouble(CA_ARSUK) +
                    Convert.ToDouble(AU_ARSUK) +
                    Convert.ToDouble(UK_NEEZ) +
                    Convert.ToDouble(DE_NEEZ) +
                    Convert.ToDouble(FR_NEEZ) +
                    Convert.ToDouble(IT_NEEZ) +
                    Convert.ToDouble(ES_NEEZ) +
                    Convert.ToDouble(NL_NEEZ) +
                    Convert.ToDouble(PL_NEEZ) +
                    Convert.ToDouble(SE_NEEZ) +
                    Convert.ToDouble(TR_NEEZ) +
                    Convert.ToDouble(USA_NEEZ) +
                    Convert.ToDouble(CA_NEEZ) +
                    Convert.ToDouble(AU_NEEZ) +
                    Convert.ToDouble(Smays_Sales) +
                    Convert.ToDouble(Salsabil) +
                    Convert.ToDouble(Cdis) +
                    Convert.ToDouble(Etsy) +
                    Convert.ToDouble(Others), 2).ToString();
                return TotalQty.ToString();
            }

        }
    }

    public class ItemSummy //For Item Report Summary Data ......
    {
        public string ItemName { get; set; }
        public string TotalAsin { get; set; }
        public string TotalSku { get; set; }
        public string TotalSold { get; set; }
        public string UKFbaStock { get; set; }
        public string EUFbaStock { get; set; }
        public string USFbaStock { get; set; }
        public string CAFbaStock { get; set; }
        public string AUFbaStock { get; set; }
                          
        public string UkWareHouse { get; set; }
        public string EUWareHouse { get; set; }
        public string USWearHouse { get; set; }
        public string AUWearHouse { get; set; }
        public string CAWearHouse { get; set; }

    }


    public class ReportSummary // Item Report Data
    {
        public string ItemTitle { get; set; }
        public string ItemID { get; set; }
        public string SKUItemID { get; set; }
        public int QTY { get; set; }
        public double ItemCost { get; set; }
        public double SalePrice { get; set; }
        public double SalesCommision { get; set; }
        public string Profit
        {
            get
            {
                if (ItemCost > 0)
                {
                    return Math.Round((SalePrice + SalesCommision) - ItemCost, 2).ToString();
                }

                return "N/A";

            }

        }
        public string ProfitPercent
        {
            get
            {
                if (ItemCost > 0)
                {
                    var profit = Math.Round((SalePrice + SalesCommision) - ItemCost, 2);

                    var percent = Math.Round((profit / (SalePrice + SalesCommision) * 100), 2);

                    return percent.ToString();
                }

                return "N/A";
            }

        }
        public string ImageUrl { get; set; }
    }

        //-----------------------Group Data -------------------//

    public class GroupData //For Sub Data Report......
        {


            public string ItemName
            { get; set; }
            public string ItemID
            { get; set; }
            public string GB
            { get; set; }
            public string FR
            { get; set; }
            public string IT
            { get; set; }
            public string DE
            { get; set; }
            public string ES
            { get; set; }
            public string NL
            { get; set; }
            public string PL
            { get; set; }
            public string IE
            { get; set; }
            public string BE
            { get; set; }

            public string GBNeez
            { get; set; }
            public string FRNeez
            { get; set; }
            public string ITNeez
            { get; set; }
            public string DENeez
            { get; set; }
            public string ESNeez
            { get; set; }
            public string NLNeez
            { get; set; }
            public string PLNeez
            { get; set; }
            public string Samaya
            { get; set; }
            public string Salsabil
            { get; set; }

            /* public string CDisc
             { get; set; }*/
            public string Etsy
            { get; set; }

            public string TotalSold
            {
                get
                {
                    var TotalQty = Math.Round(Convert.ToDouble(GB) + Convert.ToDouble(FR) + Convert.ToDouble(IT) + Convert.ToDouble(DE) + Convert.ToDouble(ES) + Convert.ToDouble(NL), 2).ToString();
                    return TotalQty.ToString();
                }

            }

            public string totalGB
            {
                get
                {

                    var total = 0;  //set a variable that holds our total
                    if (GB == null)
                    {
                        GB = "0";
                    }
                    for (int i = 0; i < GB.Length; i++)
                    {  //loop through the array
                        total += GB[i];  //Do the math!
                    }

                    return total.ToString();
                }
            }

            public string ItemCount // Summery Data 
            { get; set; }

            public string GroupName // Summery Data 
            { get; set; }


        }

    public class GroupReportModel
    {
        public string ItemMasterID { get; set; }
        public string ItemName { get; set; }
        public string UK_ARSUK { get; set; } 
        public string DE_ARSUK  {get;set;} 
        public string FR_ARSUK  {get;set;} 
        public string IT_ARSUK {get;set;} 
        public string ES_ARSUK {get;set;} 
        public string NL_ARSUK {get;set;} 
        public string PL_ARSUK {get;set;} 
        public string SE_ARSUK {get;set;} 
        public string TR_ARSUK {get;set;} 
        public string USA_ARSUK {get;set;} 
        public string CA_ARSUK {get;set;} 
        public string AU_ARSUK {get;set;}
        public string TotalARSUK {
            get
            {
                var TotalQty = Math.Round(Convert.ToDouble(UK_ARSUK) +
                    Convert.ToDouble(DE_ARSUK) +
                    Convert.ToDouble(FR_ARSUK) +
                    Convert.ToDouble(IT_ARSUK) +
                    Convert.ToDouble(ES_ARSUK) +
                    Convert.ToDouble(NL_ARSUK) +
                    Convert.ToDouble(PL_ARSUK) +
                    Convert.ToDouble(SE_ARSUK) +
                    Convert.ToDouble(TR_ARSUK) +
                    Convert.ToDouble(USA_ARSUK) +
                    Convert.ToDouble(CA_ARSUK) +
                    Convert.ToDouble(AU_ARSUK) , 2).ToString();
                return TotalQty.ToString();
            }
        }
        public string UK_NEEZ  {get;set;} 
        public string DE_NEEZ  {get;set;} 
        public string FR_NEEZ  {get;set;} 
        public string IT_NEEZ {get;set;} 
        public string ES_NEEZ {get;set;} 
        public string NL_NEEZ {get;set;} 
        public string PL_NEEZ {get;set;} 
        public string SE_NEEZ {get;set;} 
        public string TR_NEEZ {get;set;} 
        public string USA_NEEZ {get;set;} 
        public string CA_NEEZ {get;set;} 
        public string AU_NEEZ {get;set;}
        public string TotalNEEZ
        {
            get
            {
                var TotalQty = Math.Round(
                    Convert.ToDouble(UK_NEEZ) +
                    Convert.ToDouble(DE_NEEZ) +
                    Convert.ToDouble(FR_NEEZ) +
                    Convert.ToDouble(IT_NEEZ) +
                    Convert.ToDouble(ES_NEEZ) +
                    Convert.ToDouble(NL_NEEZ) +
                    Convert.ToDouble(PL_NEEZ) +
                    Convert.ToDouble(SE_NEEZ) +
                    Convert.ToDouble(TR_NEEZ) +
                    Convert.ToDouble(USA_NEEZ) +
                    Convert.ToDouble(CA_NEEZ) +
                    Convert.ToDouble(AU_NEEZ), 2).ToString();
                return TotalQty.ToString();
            }
        }
        public string Smays_Sales {get;set;} 
        public string Salsabil {get;set;} 
		public string Cdis {get;set;} 
		public string Etsy {get;set;} 
		public string Others {get;set;}
        
        public string TotalSold
        {
            get
            {
                var TotalQty = Math.Round(Convert.ToDouble(UK_ARSUK) + 
                    Convert.ToDouble(DE_ARSUK) +
                    Convert.ToDouble(FR_ARSUK) +
                    Convert.ToDouble(IT_ARSUK) + 
                    Convert.ToDouble(ES_ARSUK) +
                    Convert.ToDouble(NL_ARSUK) +
                    Convert.ToDouble(PL_ARSUK) +
                    Convert.ToDouble(SE_ARSUK) + 
                    Convert.ToDouble(TR_ARSUK) +
                    Convert.ToDouble(USA_ARSUK) +
                    Convert.ToDouble(CA_ARSUK) +
                    Convert.ToDouble(AU_ARSUK)+
                    Convert.ToDouble(UK_NEEZ) +
                    Convert.ToDouble(DE_NEEZ) +
                    Convert.ToDouble(FR_NEEZ) +
                    Convert.ToDouble(IT_NEEZ) +
                    Convert.ToDouble(ES_NEEZ) +
                    Convert.ToDouble(NL_NEEZ) +
                    Convert.ToDouble(PL_NEEZ) +
                    Convert.ToDouble(SE_NEEZ) +
                    Convert.ToDouble(TR_NEEZ) +
                    Convert.ToDouble(USA_NEEZ) +
                    Convert.ToDouble(CA_NEEZ) +
                    Convert.ToDouble(AU_NEEZ) +
                    Convert.ToDouble(Smays_Sales) +
                    Convert.ToDouble(Salsabil) +
                    Convert.ToDouble(Cdis) +
                    Convert.ToDouble(Etsy) +
                    Convert.ToDouble(Others), 2).ToString();
                return TotalQty.ToString();
            }

        }
    }

    public class GroupReportSummaryModel
    {
        public string GroupName { get; set; }
        public string ItemCount { get; set; }
        public string ASIN { get; set; }
        public string SKU { get; set; }
    }
}