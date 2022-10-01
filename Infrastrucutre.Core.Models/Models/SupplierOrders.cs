using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.Models
{
    public class SupplierOrders
    {
        [Key]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long SupplierOrderId { get; set; }
        public int ItemMasterId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string SupplierName { get; set; }
        public string BatchNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDateString { get; set; }
        public decimal TotalCVM { get; set; }
        public long TotalQty { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalBox { get; set; }
        public string PortOfLoading { get; set; }
        public string WarehouseRoot { get; set; }
        public long LeadTime { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateString { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
