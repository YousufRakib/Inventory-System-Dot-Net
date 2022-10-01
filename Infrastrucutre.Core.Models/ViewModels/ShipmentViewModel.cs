using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class ShipmentViewModel
    {
        public long ShipmentOrderId { get; set; }
        public long SupplierOrderId { get; set; }
        public long OrderId { get; set; }
        public string ShipmentTitle { get; set; }
        public string SupplierName { get; set; }
        public string ItemName { get; set; }
        public string OrderDate { get; set; }
        public decimal SupplierCost { get; set; }
        public long TotalQty { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalBox { get; set; }
        public string WarehouseRoot { get; set; }
    }
}
