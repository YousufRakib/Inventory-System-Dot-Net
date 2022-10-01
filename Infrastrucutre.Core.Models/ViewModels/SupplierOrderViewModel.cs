using Infrastrucutre.Core.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class SupplierOrderViewModel
    {
        public List<SupplierOrders> SupplierOrders { get; set; }
        public SupplierOrderSummary SupplierOrderSummary { get; set; }
    }
}
