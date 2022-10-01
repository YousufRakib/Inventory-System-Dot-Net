using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models
{
    public class ItemPriceMapper : ClassMapper<ItemPrice>
    {
        public ItemPriceMapper()
        {
            Table("ItemPrices");
            Map(i => i.ItemName).Ignore(); 
            Map(i => i.FBARootName).Ignore(); 

            AutoMap();
        }
    }
    public class ItemPrice
    {
        [Key]
        public int ItemPriceID { get; set; }
        public int ItemMasterID { get; set; }
        public int FBARootId { get; set; }
        public double Vat { get; set; }
        public double OriginalPrice { get; set; }
        public double Price { get; set; }
        public bool IsItActive { get; set; }
        
        
        //display Items
        public string ItemName { get; set; }
        public string FBARootName { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public DateTime? UpdatedDate { get; set; }

        public DateTime? CreatedDate
        {
            get; set;
        }

        public DateTime? UpdatedDate
        {
            get; set;
        }

    }

    public class ItemPriceViewModel
    {
        [Key]
        public int ItemPriceID { get; set; }
        public int ItemMasterID { get; set; }
        public int FBARootId { get; set; }
        public double Vat { get; set; }
        public double OriginalPrice { get; set; }
        public double Price { get; set; }
        public string RoundPrice
        {
            get
            {
                var RPrice = Price.ToString("0.00");
                return RPrice.ToString();
            }
        }
        public bool IsItActive { get; set; }


        //display Items
        public string ItemName { get; set; }
        public string FBARootName { get; set; }
        //public DateTime? CreatedDate { get; set; }
        //public DateTime? UpdatedDate { get; set; }

        public DateTime? CreatedDate
        {
            get; set;
        }

        public string CDate
        {
            get
            {
                var SendingDate = CreatedDate == null ? "" : Convert.ToDateTime(UpdatedDate).ToString("dd/MM/yyyy");
                return SendingDate.ToString();
            }
        }

        public DateTime? UpdatedDate
        {
            get; set;
        }

        public string UDate
        {
            get
            {
                var SendingDate = UpdatedDate == null ? "" : Convert.ToDateTime(UpdatedDate).ToString("dd/MM/yyyy");
                return SendingDate.ToString();
            }
        }

    }
}
