using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.Models
{
    public class SupplierOrderSummary
    {
        [Key]
        public long Id { get; set; }
        public long SupplierOrderId { get; set; }
        public string SupplierName { get; set; }
        public string WarehouseRoot { get; set; }
        public string ShipmentTitle { get; set; }
        public long TotalQty { get; set; }
        public decimal TotalBox { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalCVM { get; set; }
        public string DepositPercentage { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public string Currency { get; set; }
        public decimal GBPRate { get; set; }
        public string Source { get; set; }
        public DateTime? ETDDate { get; set; }
        public string ETDDateString { get; set; }
        public DateTime? DepositDate { get; set; }
        public string DepositDateString { get; set; }
        public DateTime? CutOffDate { get; set; }
        public string CutOffDateString { get; set; }
        public string PortOfLoading { get; set; }
        public string PortOfArrival { get; set; }
        public long LeadTime { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateString { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
